// Copyright (c) 2026 4F Software LLC.
// SPDX-License-Identifier: MIT

// Add an event for when the user clicks the copy prompt button.
document.addEventListener("click", async event => {
   const button = event.target.closest("[data-copy-prompt]");
   if (!button) {
      return;
   }

   const promptId = button.dataset.copyPrompt;
   const prompt = document.getElementById(promptId);
   if (!prompt) {
      return;
   }

   await navigator.clipboard.writeText(prompt.value);
   const status = document.getElementById("page-status");
   if (status) {
      status.textContent = "Prompt copied to clipboard.";
   }
});

// Wire up an event for when a user submits a form.
for (const form of document.querySelectorAll(".generated-form[data-submit-url]")) {
   form.addEventListener("submit", submitGeneratedForm);
}

async function submitGeneratedForm(event) {
   event.preventDefault();
   const form = event.currentTarget;
   const data = buildFormData(form);
   const resultPanel = document.getElementById("form-result");
   const resultContent = document.getElementById("form-result-content");

   try {
      const response = await fetch(
         form.dataset.submitUrl,
         {
            method: "POST",
            headers: {
               "Content-Type":
                  "application/json"
            },
            body: JSON.stringify(data)
         });

      const text = await response.text();

      let result;
      try {
         result = JSON.parse(text);
      }
      catch {
         result = text;
      }

      resultContent.textContent = typeof result === "string" ?
         result : JSON.stringify(result, null, 2);

      resultPanel.hidden = false;
      if (!response.ok) {
         resultPanel.classList.add("request-error");
      }
   }
   catch (error) {
      resultContent.textContent = `Request failed: ${error}`;

      resultPanel.hidden = false;
      resultPanel.classList.add("request-error");
   }
}

function buildFormData(form) {
   const formData = new FormData(form);
   const result = Object.fromEntries(formData.entries());

   for (
      const checkbox of form.querySelectorAll('input[type="checkbox"]')) {
      result[checkbox.name] = checkbox.checked;
   }

   for (const number of form.querySelectorAll('input[type="number"]')) {
      if (number.value !== "") {
         result[number.name] = Number(number.value);
      }
   }

   return result;
}