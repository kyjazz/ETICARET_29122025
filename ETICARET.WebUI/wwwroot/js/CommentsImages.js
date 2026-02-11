function imageBox(image) {
    const imageBoxElement = document.getElementById("image-box");
    if (!imageBoxElement || !image) {
        return;
    }

    imageBoxElement.src = image.src;
}

async function refreshComments() {
    const commentTab = document.getElementById("comment");
    if (!commentTab) {
        return;
    }

    const url = commentTab.dataset.url;
    if (!url) {
        return;
    }

    const response = await fetch(url, {
        method: "GET",
        headers: {
            "X-Requested-With": "XMLHttpRequest"
        }
    });

    if (!response.ok) {
        throw new Error("Yorumlar yenilenemedi.");
    }

    commentTab.innerHTML = await response.text();
}

async function sendCommentRequest(url, payload) {
    const formBody = new URLSearchParams();
    Object.keys(payload).forEach((key) => {
        formBody.append(key, payload[key]);
    });

    const response = await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8",
            "X-Requested-With": "XMLHttpRequest"
        },
        body: formBody.toString()
    });

    if (!response.ok) {
        throw new Error("İşlem başarısız oldu.");
    }

    return response.json();
}

async function doComment(button, action, commentId, selector) {
    const commentTab = document.getElementById("comment");
    if (!commentTab) {
        return;
    }

    const productId = Number(commentTab.dataset.productId);

    try {
        if (action === "new_clicked") {
            const input = document.getElementById("new_comment_text");
            if (!input || !input.value.trim()) {
                alert("Lütfen yorum girin.");
                return;
            }

            await sendCommentRequest("/Comment/Create", {
                productId,
                text: input.value.trim()
            });

            input.value = "";
            await refreshComments();
            return;
        }

        if (action === "edit_clicked") {
            const commentElement = document.querySelector(selector);
            if (!commentElement) {
                return;
            }

            const isEditMode = button.dataset.editMode === "true";

            if (!isEditMode) {
                commentElement.setAttribute("contenteditable", "true");
                commentElement.focus();
                button.dataset.editMode = "true";
                button.classList.remove("btn-warning");
                button.classList.add("btn-success");
                return;
            }

            const text = commentElement.textContent.trim();
            if (!text) {
                alert("Yorum boş olamaz.");
                return;
            }

            await sendCommentRequest("/Comment/Edit", {
                id: commentId,
                text
            });

            await refreshComments();
            return;
        }

        if (action === "delete_clicked") {
            if (!confirm("Yorumu silmek istediğinize emin misiniz?")) {
                return;
            }

            await sendCommentRequest("/Comment/Delete", {
                id: commentId
            });

            await refreshComments();
        }
    } catch (error) {
        console.error(error);
        alert("İşlem sırasında bir hata oluştu.");
    }
}