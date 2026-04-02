document.addEventListener("DOMContentLoaded", async () => {
  await bindCalender();
  loaderFadeOut();
});

const bindCalender = async () => {
  try {
    const calendar = document.getElementById("calendar");
    const monthYear = document.getElementById("monthYear");

    const today = new Date();
    const year = today.getFullYear();
    const month = today.getMonth();

    // Month names in English
    const monthNames = [
      "January",
      "February",
      "March",
      "April",
      "May",
      "June",
      "July",
      "August",
      "September",
      "October",
      "November",
      "December",
    ];

    // Set header title (e.g., April 2026)
    monthYear.innerText = `${monthNames[month]} ${year}`;

    // Get total days in current month
    const daysInMonth = new Date(year, month + 1, 0).getDate();

    for (let i = 1; i <= daysInMonth; i++) {
      const dayDiv = document.createElement("div");
      dayDiv.classList.add("day");

      const dateKey = `${year}-${month}-${i}`;
      //   const savedNote = localStorage.getItem(dateKey);
      const savedNote = await getItemData(
        `SELECT Description FROM Callenders WHERE [Key] = '${dateKey}'`,
      );

      dayDiv.innerHTML = `<div class="date">${i}</div>`;

      // If note exists, display it
      if (savedNote) {
        dayDiv.classList.add("has-note");
        const noteDiv = document.createElement("div");
        noteDiv.classList.add("note");
        noteDiv.innerText = savedNote;
        dayDiv.appendChild(noteDiv);
      }

      dayDiv.addEventListener("click", async () => {
        try {
          const existingNote =
            (await getItemData(
              `SELECT Description FROM Callenders WHERE [Key] = '${dateKey}'`,
            )) || "";

          const result = await Swal.fire({
            title: `Add a note for ${monthNames[month]} ${i}`,
            input: "textarea",
            inputValue: existingNote,
            inputPlaceholder: "Write your note here...",
            showCancelButton: true,
            confirmButtonText: "Save Note",
            cancelButtonText: "Close",
            showDenyButton: existingNote ? true : false,
            denyButtonText: "Remove Note",
            customClass: {
              popup: isDark ? "bg-dark text-white" : "bg-white text-dark",
            },
            preConfirm: (value) => {
              if (value.trim() === "") {
                Swal.showValidationMessage("Note cannot be empty!");
              }
              const forbiddenChars = /['*"&%$#]/;
              if (forbiddenChars.test(value)) {
                Swal.showValidationMessage(
                  `Special characters like ', and, %, *, " $, # are not allowed!`,
                );
                return false;
              }
              return value;
            },
          });

          if (result.isConfirmed) {
            const formData = { key: dateKey, notes: result.value };

            const response = await fetch(URIMETHOD + "/Save", {
              method: "POST",
              headers: { "Content-Type": "application/json; charset=utf-8" },
              body: JSON.stringify({ data: formData }),
            });

            if (!response.ok) {
              const errorText = await response.text();
              throw new Error(`${response.status}\n${errorText}`);
            }

            const res = await response.json();
            const dataResult = res.d || res;

            if (dataResult.error) {
              await isWarning(dataResult.error.message?.toUpperCase());
            } else {
              await isSuccess(dataResult.success);
              location.reload();
              // Optional: update UI without reload
            }
          } else if (result.isDenied) {
            const resDel = await deleteNotes(dateKey);
            if (resDel != "200") {
              throw new Error(resDel);
            } else {
              await isSuccess("Successfully removed note.");
              location.reload();
            }
          }
        } catch (err) {
          console.error(err);
          isError(err);
        }
      });

      calendar.appendChild(dayDiv);
    }
  } catch (err) {
    console.error(err);
    isError(err);
  }
};

const deleteNotes = async (key) => {
  try {
    const response = await fetch(`${URIMETHOD}/Delete`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ key: key }),
    });

    const json = await response.json();
    return json.d;
  } catch (err) {
    console.error(err);
    isError(err);
  }
};

const getItemData = async (query) => {
  try {
    const response = await fetch(`${URIMETHOD}/GetItemData`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ query: query }),
    });

    const json = await response.json();
    return json.d;
  } catch (err) {
    console.error(err);
    isError(err);
  }
};
