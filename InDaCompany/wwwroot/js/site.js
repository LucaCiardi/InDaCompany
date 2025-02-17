
document.querySelectorAll(".btnComment").forEach(btn => {
    btn.addEventListener("click", function () {
        const threadId = this.getAttribute("data-thread-id");
        document.getElementById(`comments-${threadId}`).classList.toggle("d-none");
    });
});
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll(".btnComment").forEach(btn => {
        btn.addEventListener("click", function () {
            const threadId = this.getAttribute("data-thread-id");
            document.getElementById(`comments-${threadId}`).classList.toggle("d-none");
        });
    });
});
function toggleLike(threadId) {
    $.post('/Like/ToggleLike', { threadId: threadId })
        .done(function (response) {
            if (response.success) {
                const likeButton = $(`.likeButton[data-thread-id="${threadId}"]`);
                const likeCount = $(`#likeCount-${threadId}`);

                if (response.liked) {
                    likeButton.addClass('active');
                } else {
                    likeButton.removeClass('active');
                }

                likeCount.text(response.likeCount);
            }
        });
}

$(document).ready(function () {
    $('.likeButton').each(function () {
        const threadId = $(this).data('thread-id');
        $.get(`/Like/GetLikeStatus?threadId=${threadId}`)
            .done(function (response) {
                if (response.success) {
                    const likeButton = $(`.likeButton[data-thread-id="${threadId}"]`);
                    const likeCount = $(`#likeCount-${threadId}`);

                    if (response.liked) {
                        likeButton.addClass('active');
                    }
                    likeCount.text(response.likeCount);
                }
            });
    });
});
