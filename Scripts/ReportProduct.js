document.addEventListener("DOMContentLoaded", () => {
  console.log("ReportProduct.js loaded successfully");
  reportProductPageLoaded();
});
// =================================================EVENTS==================================================
document
  .querySelectorAll("#cardFind .form-control, #cardFind .form-select")
  .forEach((el) => {
    el.addEventListener("change", async (e) => {
      e.target.classList.remove("is-invalid");

      if (e.target.id === "findby") {
        const finedlabel = document.getElementById("finedLabel");
        const findby = e.target.value;
        finedlabel.innerText = findby || "Fined";
        await bindFined(findby);
      }
    });
    el.addEventListener("input", (e) => {
      e.target.classList.remove("is-invalid");
    });
  });
// =================================================FUNCTION================================================
// --------------------------------------------||Binding Function||-----------------------------------------
const binFindBy = () => {
  const sel = document.getElementById("findby");
  sel.innerHTML = ""; //reset

  let data = [];
  let list = [];

  list = ["product", "customer"];

  list.forEach((ls) => {
    data.push({ value: ls, text: ls });
  });

  if (data.length > 1) {
    const defaultOption = document.createElement("option");
    defaultOption.text = "";
    defaultOption.value = "";
    sel.add(defaultOption);
  }

  data.forEach((item) => {
    const option = document.createElement("option");
    option.value = item.value;
    option.text = item.text.toUpperCase();
    option.setAttribute("data-name", item.text);
    sel.add(option);
  });
};

const bindDate = () => {
  const fromdate = document.getElementById("fromdate");
  const todate = document.getElementById("todate");

  fromdate.value = new Date().toISOString().split("T")[0];
  todate.value = new Date().toISOString().split("T")[0];
};

const bindFined = async (findby) => {
  const select = document.getElementById("fined");
  select.innerHTML = "";

  if (!findby) return;

  try {
    const response = await fetch(`${URIMETHOD}/BindListData`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
      body: JSON.stringify({
        data: {
          field: "fined",
          findby,
        },
      }),
    });

    if (!response.ok) {
      const text = await response.text();
      const msg = `${response.status}\n${text}`;
      throw new Error(msg);
    }

    // parsing hasil response JSON
    const result = await response.json();
    const data = result.d;

    // validasi apakah ada data
    if (!data) {
      throw new Error("No data returned from server : bindFined");
    }

    // render ke elemen halaman
    if (Array.isArray(data)) {
      select.innerHTML = ""; //reset

      if (data.length > 1) {
        const defaultOption = document.createElement("option");
        defaultOption.text = "ALL";
        defaultOption.value = "";
        select.add(defaultOption);
      }

      data.forEach(function (item) {
        const option = document.createElement("option");
        option.value = item.value;
        option.text = item.text.toUpperCase();
        option.setAttribute("data-name", item.text);
        select.add(option);
        select.classList.add("fw-bold");
      });

      if (data.length === 1) {
        select.selectedIndex = 0;
      }
    }
  } catch (err) {
    const msg =
      ROLENAME === "Administrator"
        ? err.message
        : "Please contact our IT team at support@onlineorder.au";
    isError(msg);
  }
};

const reportProductPageLoaded = async () => {
  await Promise.all([binFindBy(), bindDate()]);
  await loaderFadeOut();
};
