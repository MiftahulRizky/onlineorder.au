document.addEventListener("DOMContentLoaded", () => {
  console.log("order-detail.js loaded successfully");
  // Reset MSG Success
  resetMSGSuccess();
});

// ======================================EVENTS=====================================
// ======================================FUNCTIONS==================================
function resetMSGSuccess() {
  window.setTimeout(() => {
    const alertSuccess = document.querySelector(".alert-success");
    if (alertSuccess) {
      alertSuccess.style.transition = "opacity 0.5s ease";
      alertSuccess.style.opacity = "0";

      setTimeout(() => {
        alertSuccess.style.transition = "all 0.5s ease";
        alertSuccess.style.height = "0";
        alertSuccess.style.margin = "0";
        alertSuccess.style.padding = "0";

        setTimeout(() => {
          alertSuccess.remove();
        }, 500);
      }, 500);
    }
  }, 3000);
}
