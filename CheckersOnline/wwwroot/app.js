document.addEventListener("DOMContentLoaded", () => {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/gameHub")
        .build();
    
    const findGameBtn = document.getElementById("btn-find-game");
    const concedeBtn = document.getElementById("btn-concede");
    const rematchBtn = document.getElementById("btn-rematch");
    const mainMenu1Btn = document.getElementById("btn-main-menu-1");
    const mainMenu2Btn = document.getElementById("btn-main-menu-2");
    
    const statusText = document.getElementById("status");
    const board = document.getElementById("game-board");
    
    let groupId;
    let playerColor;
    
    connection.serverTimeoutInMilliseconds = 10000;
    connection.start().catch(err => console.error(err));
    
    window.addEventListener("beforeunload", () => {
        connection.invoke("UserLeaving");
    })
    
    findGameBtn.addEventListener("click", () => {
        connection.invoke("QuickGame").catch(err => console.error(err));
    
        findGameBtn.classList.add("hidden"); // Прячем кнопку
        document.getElementById("loader").classList.remove("hidden"); // Показываем спиннер
    });
    
    concedeBtn.addEventListener("click", () => {
        connection.invoke("Concede", groupId).catch(err => console.error(err));
    })
    
    rematchBtn.addEventListener("click", () => {
        connection.invoke("Rematch", groupId).catch(err => console.error(err));
    })

    mainMenu1Btn.addEventListener("click", () => {
        connection.invoke("NoRematch", groupId).catch(err => console.error(err));
        console.log("No rematch was sent");
        resetLobby();
    })
    
    mainMenu2Btn.addEventListener("click", () => {
        resetLobby();
    })
    
    // Сервер сообщает, что мы в очереди
    connection.on("WaitingForOpponent", () => {
        if (rematchBtn) {
            rematchBtn.innerText = "Ожидание оппонента...";
            rematchBtn.disabled = true; // Блокируем кнопку
            rematchBtn.style.opacity = "0.7"; // Визуально приглушаем
            rematchBtn.style.cursor = "not-allowed";
        }
    });
    
    // Сервер сообщает, что игра найдена
    connection.on("GameStarted", (gId) => {
        groupId = gId;
        document.getElementById("lobby").classList.add("hidden");
        document.getElementById("game-area").classList.remove("hidden");
        
        hideGameModal()
        createBoard();
    });
    
    connection.on("AllPositions", (message) =>{
        console.log(message);
        renderPieces(message.split(' '))
    })
    
    connection.on("EnemyConceded", (score) => {
        console.log("Enemy Conceded!");
        
        const parts = score.split("|");
        showGameModal(parts[0], parts[1]);
    })
    
    connection.on("YouConceded", (score) =>{
        console.log("You conceded... skill issue?");

        const parts = score.split("|");
        showGameModal(parts[0], parts[1]);
    })
    
    connection.on("WaitingForRematch", () =>{
        if (rematchBtn) {
            rematchBtn.innerText = "Ожидание оппонента...";
            rematchBtn.disabled = true; // Блокируем кнопку
            rematchBtn.style.opacity = "0.7"; // Визуально приглушаем
            rematchBtn.style.cursor = "not-allowed";
        }
    })
    
    
    connection.on("OpponentDisconnected", () => {
        console.log("Opponent disconnected... skill issue? ");
        
        const modal = document.getElementById('modal-overlay');
        modal.classList.remove("hidden");
        
        document.getElementById("modal-content-default").classList.add("hidden");
        document.getElementById("btn-rematch").classList.add("hidden");
        
        const disconnectBlock = document.getElementById("modal-content-disconnect");
        disconnectBlock.classList.remove("hidden");
        
        modal.querySelector("h2").innerText = "Оппонент отключился";
    })
    
    
    function createBoard() {
        board.innerHTML = ""; // Очистка
        for (let row = 0; row < 8; row++) {
            for (let col = 0; col < 8; col++) {
                const cell = document.createElement("div");
                cell.classList.add("cell");
                cell.dataset.row = row;
                cell.dataset.col = col;
                
                if ((row + col) % 2 === 0) {
                    cell.classList.add("white");
                } else {
                    cell.classList.add("black");
                }
                board.appendChild(cell);
            }
        }
        connection.invoke("GetAllPositions", groupId).catch(err => console.error(err));
    }
    
    
    function renderPieces(pieces) {
        document.querySelectorAll('.piece').forEach(p => p.remove());
    
        pieces.forEach(p => {
            const parts = p.split("|");
            const x = parts[0];
            const y = parts[1];
            const color = parts[2];
            const isKing = parts[3] == 1;
            
            const cell = document.querySelector(`.cell[data-row="${x}"][data-col="${y}"]`);
    
            if (cell) {
                const pieceDiv = document.createElement('div');
                pieceDiv.classList.add('piece');
                pieceDiv.classList.add(`${(color == 0 ? "white": "black")}-piece`);
    
                if (isKing) {
                    pieceDiv.classList.add('king');
                }
                cell.appendChild(pieceDiv);
            }
        });
    }
    
    function showGameModal(playerScore, opponentScore) {
        document.getElementById("player-score").innerText = playerScore;
        document.getElementById("opponent-score").innerText = opponentScore;
        
        const modal = document.getElementById('modal-overlay');
        modal.classList.remove('hidden');
    }
    
    function hideGameModal(){
        const modal = document.getElementById('modal-overlay');
        modal.classList.add('hidden');

        if (rematchBtn) {
            rematchBtn.innerText = "Реванш";
            rematchBtn.disabled = false;
            rematchBtn.style.opacity = "1";
            rematchBtn.style.cursor = "pointer";
        }
    }
    
    function resetLobby() {
        document.getElementById("modal-overlay").classList.add("hidden");
        document.getElementById("game-area").classList.add("hidden");
        
        document.getElementById("lobby").classList.remove("hidden");
        document.getElementById("loader").classList.add("hidden");
        findGameBtn.classList.remove("hidden");

        document.getElementById("modal-content-default").classList.remove("hidden");
        document.getElementById("modal-content-disconnect").classList.add("hidden");
        document.getElementById("btn-rematch").classList.remove("hidden");
        document.querySelector('.modal-card h2').innerText = "Игра окончена";
    }
})

