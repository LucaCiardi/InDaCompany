
document.querySelectorAll(".btnComment").forEach(btn => {
    btn.addEventListener("click", function () {
        const threadId = this.getAttribute("data-thread-id");
        document.getElementById(`comments-${threadId}`).classList.toggle("d-none");
    });
});
