console.log("site.js が読み込まれました");

document.addEventListener('DOMContentLoaded', () => {

    // ===== 遊び方モーダル =====
    const howtosign = document.getElementById('howToPlaySign');
    const howtomodal = document.getElementById('howtoModal');
    const howtocloseBtn = document.getElementById('closeHowToPlay');

    // 1. if文のあとに { を入れる
    if (howtosign && howtomodal && howtocloseBtn) {

        // 2. style.display の書き方を修正
        window.showHowtoModal = () => {
            console.log("モーダルを表示します");
            howtomodal.style.display = 'flex'; // シンプルにこれでOK
        };

        window.closeHowtoModal = () => {
            howtomodal.style.display = 'none';
        };

        // イベント登録
        howtosign.addEventListener('click', (e) => {
            e.stopPropagation();
            window.showHowtoModal();
        });

        howtocloseBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            window.closeHowtoModal();
        });

        howtomodal.addEventListener('click', (e) => {
            if (e.target === howtomodal) window.closeHowtoModal();
        });
    } // 3. 最後に } で閉じる

    // ===== 羊モーダル =====
    const sheepmodal = document.getElementById('sheepModal');
    const sheepmodalText = document.getElementById('sheepModalText');
    const okButton = document.getElementById('okButton');
    const deleteButton = document.getElementById('deleteButton');

    window.showSheepModal = (name, id) => {
        if (!sheepmodal || !sheepmodalText) return;
        sheepmodalText.innerText = `${name} です`;
        sheepmodal.dataset.sheepId = id;
        sheepmodal.style.display = 'flex';
    };

    if (sheepmodal) {
        okButton.onclick = (e) => {
            e.stopPropagation();
            sheepmodal.style.display = 'none';
        };
        deleteButton.onclick = async (e) => {
            e.stopPropagation();
            const id = sheepmodal.dataset.sheepId;
            const sheepEl = document.querySelector(`.sheep[data-id="${id}"]`);
            try {
                const response = await fetch(`/sheeps/Delete?id=${id}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                });
                if (!response.ok) throw new Error('削除失敗');
                if (sheepEl) sheepEl.remove();
                sheepmodal.style.display = 'none';
                alert('削除しました');
            } catch (err) {
                alert('削除に失敗しました');
            }
        };
    }

    // ===== クイズ判定 =====

    function fitTextToSign(signEl) {
        if (!signEl) return;

        let fontSize = parseFloat(getComputedStyle(signEl).fontSize);
        const MIN_FONT = 14;

        while (
            signEl.scrollHeight > signEl.clientHeight &&
            fontSize > MIN_FONT
        ) {
            fontSize -= 1;
            signEl.style.fontSize = fontSize + 'px';
        }
    }


    const today = new Date().toISOString().slice(0, 10);
    const quizStorageKey = "sheepQuizDone";
    const now = new Date();
    const quizActive = (now.getHours() >= 18) && (localStorage.getItem(quizStorageKey) !== today);

    const sheeps = document.querySelectorAll('.sheep');
    let correctSheep = null;

    if (quizActive && sheeps.length > 0) {
        const index = Math.floor(Math.random() * sheeps.length);
        correctSheep = sheeps[index];
        const hintDiv = document.getElementById('quizHint');
        if (correctSheep.dataset.name && hintDiv) {
            hintDiv.innerText = `羊を探そう\n${correctSheep.dataset.name} はどこ？`;
        }
    }

    fitTextToSign(document.getElementById('quizSign'))

    window.checkSheep = (el) => {
        // 羊要素以外は無視
        if (!el.classList.contains('sheep')) return;

        const name = el.dataset.name || "名前不明";
        const id = el.dataset.id;

        if (!quizActive) {
            window.showSheepModal(name, id);
        } else {
            const resultDiv = document.getElementById('quizResult');
            if (el === correctSheep) {
                resultDiv.innerText = '正解！🎉';
                //localStorage.setItem(quizStorageKey, today);
            } else {
                resultDiv.innerText = '';
                window.showSheepModal(name, id);
            }
        }
    };

    // ===== 羊アニメーション =====
    const sheepField = document.getElementById('sheepField');

    if (!sheepField || sheeps.length === 0) throw new Error("sheepField or sheeps not found");

    // 初期化関数
    function getSheepSize() {
        return window.innerWidth <= 768 ? 80 : 240;
    }

    let SHEEP_SIZE = getSheepSize();

    // フィールドサイズを取得
    function getFieldSize() {
        return {
            width: sheepField.clientWidth,
            height: sheepField.clientHeight
        };
    }

    let fieldSize = getFieldSize();

    // 初期の羊データ
    let sheepData = [];

    function initSheep() {
        SHEEP_SIZE = getSheepSize();
        fieldSize = getFieldSize();

        sheepData = Array.from(sheeps).map(el => {
            const x = Math.random() * (fieldSize.width - SHEEP_SIZE);
            const y = Math.random() * (fieldSize.height - SHEEP_SIZE);
            const SPEED_MULTIPLIER = window.innerWidth <= 768 ? 1 : 2;

            el.style.transform = `translate(${x}px, ${y}px)`;

            function randomSpeed() {
                const base = Math.random() * 0.5 + 0.25;
                const dir = Math.random() < 0.5 ? -1 : 1;
                return base * SPEED_MULTIPLIER * dir;
            }

             return {
                    el,x, y,
                    dx: randomSpeed(),
                     dy: randomSpeed()
                };
        });
    }

    // resize対応
    window.addEventListener('resize', () => {
        SHEEP_SIZE = getSheepSize();
        fieldSize = getFieldSize();

        // 再配置（必要なら）
        sheepData.forEach(s => {
            // x/yが新しいサイズを超えていたら調整
            s.x = Math.min(s.x, fieldSize.width - SHEEP_SIZE);
            s.y = Math.min(s.y, fieldSize.height - SHEEP_SIZE);
            s.el.style.transform = `translate(${s.x}px, ${s.y}px)`;
        });
    });

    // アニメーション
    function animate() {
        const maxX = fieldSize.width - SHEEP_SIZE;
        const maxY = fieldSize.height - SHEEP_SIZE;

        sheepData.forEach(s => {
            s.x += s.dx;
            s.y += s.dy;

            if (s.x <= 0 || s.x >= maxX) s.dx *= -1;
            if (s.y <= 0 || s.y >= maxY) s.dy *= -1;

            s.el.style.transform = `translate(${s.x}px, ${s.y}px)`;
        });

        requestAnimationFrame(animate);
    }

    // 初期化と開始
    initSheep();
    animate();

});
