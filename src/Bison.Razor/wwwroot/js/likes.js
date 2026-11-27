// Handles clicks on the thumbs-up buttons rendered by Pages/Shared/_LikeButton.cshtml.
// Signed-in users toggle the like with a POST to /ob/{id}?handler=Like (ObservationModel.OnPostLike),
// signed-out users are sent to the login page.
(function () {
  function loginUrl() {
    var returnUrl = window.location.pathname + window.location.search;
    return "/Identity/Account/Login?returnUrl=" + encodeURIComponent(returnUrl);
  }

  function antiForgeryToken() {
    var input = document.querySelector('input[name="__RequestVerificationToken"]');
    return input ? input.value : "";
  }

  function render(button, liked, likes) {
    button.classList.toggle("liked", liked);
    button.setAttribute("aria-pressed", liked ? "true" : "false");
    button.querySelector(".like-count").textContent = likes;
  }

  document.addEventListener("click", async function (event) {
    var button = event.target.closest(".like-button");
    if (!button) return;
    event.preventDefault();

    if (document.body.dataset.signedIn !== "true") {
      window.location.href = loginUrl();
      return;
    }

    if (button.disabled) return;
    button.disabled = true;
    try {
      var id = button.dataset.observationId;
      var response = await fetch("/ob/" + encodeURIComponent(id) + "?handler=Like", {
        method: "POST",
        headers: { RequestVerificationToken: antiForgeryToken() },
        credentials: "same-origin",
      });

      if (response.status === 401) {
        window.location.href = loginUrl();
        return;
      }
      if (!response.ok) return;

      var result = await response.json();
      // The same observation can be shown more than once on a page, so update every button for it.
      document
        .querySelectorAll('.like-button[data-observation-id="' + id + '"]')
        .forEach(function (b) {
          render(b, result.liked, result.likes);
        });
    } finally {
      button.disabled = false;
    }
  });
})();
