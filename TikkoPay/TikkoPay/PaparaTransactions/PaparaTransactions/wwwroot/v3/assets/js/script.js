// Initialize Bootstrap tooltips
const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl));

function copyText(button) {
    const input = button.parentElement.querySelector('input');
    input.select();
    document.execCommand('copy');
    
    // Visual feedback
    button.classList.add('copied');
    button.querySelector('i').classList.remove('bi-clipboard');
    button.querySelector('i').classList.add('bi-clipboard-check');
    
    setTimeout(() => {
        button.classList.remove('copied');
        button.querySelector('i').classList.remove('bi-clipboard-check');
        button.querySelector('i').classList.add('bi-clipboard');
    }, 1000);
}

// Add click event listeners to copy buttons
document.querySelectorAll('.copy-btn').forEach(button => {
    button.addEventListener('click', () => copyText(button));
});
