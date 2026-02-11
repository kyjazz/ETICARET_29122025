(function () {
    function getCommentPane() {
        return document.getElementById("comment");
    }

    function getProductId() {
        var pane = getCommentPane();
        return pane ? parseInt(pane.dataset.productId || "0", 10) : 0;
    }

    function getCommentUrl() {
        var pane = getCommentPane();
        return pane ? pane.dataset.url : null;
    }

    function refreshComments() {
        var pane = getCommentPane();
        var url = getCommentUrl();
        if (!pane || !url) {
            return;
        }

        fetch(url, {
            method: "GET",
            headers: { "X-Requested-With": "XMLHttpRequest" }
        })
            .then(function (response) { return response.text(); })
            .then(function (html) {
                pane.innerHTML = html;
            });
    }

    function postJson(url, data) {
        return fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "X-Requested-With": "XMLHttpRequest"
            },
            body: JSON.stringify(data)
        }).then(function (response) { return response.json(); });
    }

    window.doComment = function (button, mode, commentId, elementSelector) {
        var productId = getProductId();

        if (mode === "new_clicked") {
            var input = document.getElementById("new_comment_text");
            var text = input ? input.value.trim() : "";
            if (!text) {
                alert("Lütfen yorum metni giriniz.");
                return;
            }

            postJson("/Comment/Add", { text: text, productId: productId })
                .then(function (result) {
                    if (!result.result) {
                        alert(result.message || "Yorum eklenemedi.");
                        return;
                    }

                    input.value = "";
                    refreshComments();
                });

            return;
        }

        if (mode === "delete_clicked") {
            if (!confirm("Yorumu silmek istediğinize emin misiniz?")) {
                return;
            }

            postJson("/Comment/Remove", { id: commentId })
                .then(function (result) {
                    if (!result.result) {
                        alert(result.message || "Yorum silinemedi.");
                        return;
                    }

                    refreshComments();
                });

            return;
        }

        if (mode === "edit_clicked") {
            var textElement = document.querySelector(elementSelector);
            if (!textElement) {
                return;
            }

            var isEditMode = button.dataset.editMode === "true";

            if (!isEditMode) {
                button.dataset.editMode = "true";
                button.innerHTML = '<span class="fas fa-save fa-xs"></span>';
                textElement.contentEditable = "true";
                textElement.focus();
                return;
            }

            var text = textElement.innerText.trim();
            if (!text) {
                alert("Yorum metni boş olamaz.");
                return;
            }

            postJson("/Comment/Update", { id: commentId, text: text, productId: productId })
                .then(function (result) {
                    if (!result.result) {
                        alert(result.message || "Yorum güncellenemedi.");
                        return;
                    }

                    button.dataset.editMode = "false";
                    button.innerHTML = '<span class="fas fa-edit fa-xs"></span>';
                    textElement.contentEditable = "false";
                    refreshComments();
                });
        }
    };

    window.imageBox = function (imgElement) {
        var imageBoxElement = document.getElementById("image-box");
        if (imageBoxElement && imgElement) {
            imageBoxElement.src = imgElement.src;
        }
    };
})();