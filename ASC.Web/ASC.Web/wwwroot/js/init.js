// Chặn nút back/forward sau khi đăng xuất
window.history.forward();

function noBack() {
    window.history.forward();
}

window.onload = noBack;
window.onpageshow = function (event) {
    if (event.persisted) {
        noBack();
    }
};

// Chặn chuột phải
document.addEventListener("contextmenu", function (event) {
    event.preventDefault();
});