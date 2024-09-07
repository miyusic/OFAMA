// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
    // 現在のURLからページの識別子を取得
    const path = window.location.pathname.split('/');
    const currentPage = path[1] || 'Home'; // ルートの場合は'Home'をデフォルト値とする

    // すべてのナビゲーションリンクを取得（.nav-linkと.dropdown-itemの両方を含む）
    const navLinks = document.querySelectorAll('.nav-link[data-target], .dropdown-item[data-target]');

    // navLinksの内容をコンソールに出力
    // console.log(navLinks);

    // 各ナビゲーションリンクに対して処理を実行
    navLinks.forEach(link => {
        // リンクのdata-target属性を取得
        const targetPage = link.getAttribute('data-target');
        // リンクの親要素がドロップダウンメニューの場合、親リンクを取得
        const parentLink = link.closest('.dropdown-menu')?.previousElementSibling;

        // 現在のページがリンクのtargetと一致する場合、'active'クラスを追加
        if (targetPage === currentPage) {
            link.classList.add('active');
            parentLink.classList.add('active');
            console.log("ok")
            if (parentLink && parentLink.classList.contains('nav-link') && parentLink.classList.contains('dropdown-toggle')) {
                parentLink.classList.add('active');
            }
        } else {
            // 一致しない場合は'active'クラスを削除
            link.classList.remove('active');
            if (parentLink && parentLink.classList.contains('nav-link') && parentLink.classList.contains('dropdown-toggle')) {
                parentLink.classList.remove('active');
            }
        }
    });

    // 特定のボタンがクリックされたときに、すべてのナビゲーションアイテムから'active'クラスを削除
    const navigateButton = document.getElementById('navigateButton');
    if (navigateButton) {
        navigateButton.addEventListener('click', function () {
            navLinks.forEach(link => {
                link.classList.remove('active');
                const parentLink = link.closest('.dropdown-menu')?.previousElementSibling;
                if (parentLink && parentLink.classList.contains('nav-link') && parentLink.classList.contains('dropdown-toggle')) {
                    parentLink.classList.remove('active');
                }
            });
        });
    }
});
