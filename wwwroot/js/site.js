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
    const today = new Date().toISOString().slice(0, 10);
    const quizStorageKey = "sheepQuizDone";
    const now = new Date();
    const quizActive = (now.getHours() >= 21) && (localStorage.getItem(quizStorageKey) !== today);

    const sheeps = document.querySelectorAll('.sheep');
    let correctSheep = null;

    if (quizActive && sheeps.length > 0) {
        const index = Math.floor(Math.random() * sheeps.length);
        correctSheep = sheeps[index];
        const hintDiv = document.getElementById('quizHint');
        if (correctSheep.dataset.name && hintDiv) {
            hintDiv.innerText = `${correctSheep.dataset.name} はどこ？`;
        }
    }

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
                localStorage.setItem(quizStorageKey, today);
            } else {
                resultDiv.innerText = '';
                window.showSheepModal = (name, id) => {
                    if (!sheepmodal || !sheepmodalText) return;
                    sheepmodalText.innerText = `${name} です`;
                    sheepmodal.dataset.sheepId = id;
                    sheepmodal.style.display = 'flex';
                };
            }
        }
    };

    // ===== 羊アニメーション =====
    const sheepField = document.getElementById('sheepField');
    if (sheepField && sheeps.length > 0) {
        const SHEEP_SIZE = 300;

        const sheepData = Array.from(sheeps).map(el => {
            const x = Math.random() * (sheepField.clientWidth - SHEEP_SIZE);
            const y = Math.random() * (sheepField.clientHeight - SHEEP_SIZE);
            el.style.transform = `translate(${x}px, ${y}px)`;
            return {
                el, x, y,
                dx: (Math.random() * 0.5 + 0.25) * (Math.random() < 0.5 ? 1 : -1),
                dy: (Math.random() * 0.5 + 0.25) * (Math.random() < 0.5 ? 1 : -1)
            };
        });

        function animate() {
            const maxX = sheepField.clientWidth - SHEEP_SIZE;
            const maxY = sheepField.clientHeight - SHEEP_SIZE;

            sheepData.forEach(s => {
                s.x += s.dx;
                s.y += s.dy;
                if (s.x <= 0 || s.x >= maxX) s.dx *= -1;
                if (s.y <= 0 || s.y >= maxY) s.dy *= -1;
                s.el.style.transform = `translate(${s.x}px, ${s.y}px)`;
            });

            requestAnimationFrame(animate);
        }

        animate();
    }

});
