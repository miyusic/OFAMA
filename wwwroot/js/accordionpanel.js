


$(document).ready(function () {
    // ページ読み込み時にすべてのアコーディオンボックスを閉じる
    $(".accordionbox").hide(); // アコーディオンボックスを非表示にする

    // アコーディオンをクリックした時の動作
    $(".accordiontitle").on("click", function () {
        // タイトル要素をクリックしたら
        var findElm = $(this).next(".accordionbox"); // 直後のアコーディオンを行うエリアを取得し
        $(findElm).slideToggle(); // アコーディオンの上下動作

        if ($(this).hasClass("close")) {
            // タイトル要素にクラス名closeがあれば
            $(this).removeClass("close"); // クラス名を除去し
        } else {
            // それ以外は
            $(this).addClass("close"); // クラス名closeを付与
        }
    });
});