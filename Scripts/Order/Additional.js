document.getElementById("modalSuccess").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalError").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

$(document).ready(function () {
    checkSession();

    $("#submit").on("click", proccess);
    $("#cancel").on("click", () => window.location.href = "/order/detail/");
    $("#vieworder").on("click", () => window.location.href = "/order/detail");

    $("#blindtype").on("change", function () {
        const blindType = $(this).val();
        bindProduct(blindType);
    });

    $("#product").on("change", function () {
        const blindtype = document.getElementById("blindtype").value;
        bindComponentForm(blindtype, $(this).val());
    });
});

function checkSession() {
    if (!headerId) {
        window.location.href = "/order";
        return;
    }
    if (!itemAction || !designId) {
        window.location.href = "/order/detail";
        return;
    }
    if (designId.toUpperCase() !== designIdOri) {
        window.location.href = "/order/detail";
        return;
    }

    loader(itemAction);
    getDesignName(designId);
    bindDataHeader(headerId);
    bindFormAction(itemAction);    

    if (itemAction === "AddItem") {
        bindBlindType(designId);
        controlForm(false);
        bindComponentForm("", "");
    } else if (["EditItem", "ViewItem", "CopyItem"].includes(itemAction)) {
        bindItemOrder(itemId);
        controlForm(itemAction === "ViewItem", itemAction === "EditItem", itemAction === "CopyItem");
    }
}

function loader(itemAction) {
    if (itemAction === "AddItem") {
        document.getElementById("divLoader").style.display = "none";
        document.getElementById("divOrder").style.display = "";
    } else {
        setTimeout(function () {
            document.getElementById("divLoader").style.display = "none";
            document.getElementById("divOrder").style.display = "";
        }, 3000);
    }
}

function isError(msg) {
    $("#modalError").modal("show");
    document.getElementById("errorMsg").innerHTML = msg;
}

function getDesignName(designId) {
    const pageTitle = document.getElementById("pageTitle");
    pageTitle.textContent = "";

    if (!designId) return;

    const type = "DesignName";
    $.ajax({
        type: "POST",
        url: "Method.aspx/StringData",
        data: JSON.stringify({ type: type, dataId: designId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const designName = response.d.trim() || "Nama desain tidak ditemukan";
            pageTitle.textContent = designName;
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", error, xhr.responseText);
        }
    });
}

function getBlindName(blindId) {
    if (!blindId) return;

    const type = "BlindName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: blindId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                resolve(response.d);
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
                reject(error);
            }
        });
    });
}

function getProductName(productId) {
    if (!productId) return;

    const type = "ProductName";
    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/StringData",
            data: JSON.stringify({ type: type, dataId: productId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                resolve(response.d);
            },
            error: function (xhr, status, error) {
                console.error("Error:", error);
                reject(error);
            }
        });
    });
}

function bindDataHeader(headerId) {
    $.ajax({
        type: "POST",
        url: "Method.aspx/GetDataHeader",
        data: JSON.stringify({ headerId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const data = response.d;
            if (data) {
                document.getElementById("orderid").innerText = data.OrderId || "-";
                document.getElementById("orderno").innerText = data.OrderNumber || "-";
                document.getElementById("ordername").innerText = data.OrderName || "-";
            }
        },
        error: function (xhr, status, error) {
            //console.error("AJAX Error:", error);
        }
    });
}

function bindFormAction(itemAction) {
    const cardTitle = document.getElementById("cardTitle");
    if (!cardTitle) return console.warn("Elemen 'cardTitle' tidak ditemukan.");

    const actionMap = {
        AddItem: "ADD ITEM",
        EditItem: "EDIT ITEM",
        ViewItem: "VIEW ITEM",
        CopyItem: "COPY ITEM"
    };
    cardTitle.innerText = actionMap[itemAction] || "";
}

function bindBlindType(designId) {
    return new Promise((resolve, reject) => {
        const blindtype = document.getElementById("blindtype");
        blindtype.innerHTML = "";

        if (!designId) {
            resolve();
            return;
        }

        const listData = { type: "BlindType", designtype: designId };
        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    blindtype.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        blindtype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        blindtype.add(option);
                    });

                    if (response.d.length === 1) {
                        blindtype.selectedIndex = 0;
                    }

                    const selectedValue = blindtype.value;
                    Promise.all([
                        bindProduct(selectedValue)
                    ]).then(resolve).catch(reject);
                } else {
                    resolve();
                }
            },
            error: function (xhr, status, error) {
                console.log("Terjadi kesalahan saat memanggil WebMethod: " + error);
                reject(error);
            }
        });
    });
}

function bindProduct(blindType) {
    return new Promise((resolve, reject) => {
        const product = document.getElementById("product");
        product.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        const listData = { type: "Products", blindtype: blindType };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    product.innerHTML = "";
                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        product.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        product.add(option);
                    });

                    let selectedValue = "";
                    if (response.d.length === 1) {
                        product.selectedIndex = 0;
                        selectedValue = product.value;
                        
                    }
                    bindComponentForm(blindType, selectedValue);
                } else {
                    resolve();
                }
            },
            error: function (xhr, status, error) {
                console.log("Terjadi kesalahan saat memanggil WebMethod: " + error);
            }
        });
    }); 
}

function bindComponentForm(blindType, product) {
    const divType = document.getElementById("divType");
    const divQty = document.getElementById("divQty");
    const divItemNumber = document.getElementById("divItemNumber");
    const divCost = document.getElementById("divCost");
    const divItemName = document.getElementById("divItemName");
    const divItemCode = document.getElementById("divItemCode");

    divType.style.display = "none";
    divQty.style.display = "none";
    divItemNumber.style.display = "none";
    divCost.style.display = "none";
    divItemName.style.display = "none";
    divItemCode.style.display = "none";

    if (!blindType) return;

    getBlindName(blindType).then(blindName => {
        if (blindName === "Freight") {
            divType.style.display = "";
            $("#producttitle").text("FREIGHT TYPE");
        } else if (blindName === "Freight" || blindName === "Check Measure" || blindName === "Installation" || blindName === "Takedown" || blindName === "Travel Charge") {
            divType.style.display = "";
            $("#producttitle").text("TYPE");
        } else if (blindName === "Parts") {
            divItemName.style.display = "";
            divItemCode.style.display = "";
            divQty.style.display = "";
        }

        if (!product) return;

        let titleCost = document.getElementById("costtitle");
        getProductName(product).then(productName => {
            if (productName === "Template Fee" || productName === "Blind Products" || productName === "Concrete/Brick" || productName === "Program Hub" || productName === "Shutters" || productName === "Blinds") {
                divItemNumber.style.display = "";
            }
            if (productName === "Misc. Travel Charge" || productName === "Other" || productName === "TAS - NT - WA" || productName === "Curtain" || productName === "Parts") {
                divCost.style.display = "";
                titleCost.innerHTML = "PRICE PER M2";
                if (productName === "Misc. Travel Charge" || productName === "Curtain" || productName === "Parts") {
                    titleCost.innerHTML = "UNIT PRICE";
                }
            }
        }).catch(error => {
            console.error("Gagal mendapatkan fabric name:", error);
        });
    }).catch(error => {
        console.error("Gagal mendapatkan fabric name:", error);
    });
}

function proccess() {
    toggleButtonState(true, "Processing...");

    const fields = ["blindtype", "product", "itemname", "itemcode", "qty", "itemnumber", "cost"];

    const formData = {
        headerid: headerId,
        itemaction: itemAction,
        itemid: itemId,
        designid: designId,
        loginid: loginId
    };

    fields.forEach(id => {
        formData[id] = document.getElementById(id).value;
    });

    $.ajax({
        type: "POST",
        url: "Method.aspx/AdditionalProccess",
        data: JSON.stringify({ data: formData }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const result = response.d.trim();
            if (result === "Success") {
                setTimeout(() => {
                    $('#modalSuccess').modal('show');
                    startCountdown(3);
                }, 1000);
            } else {
                isError(result);
                toggleButtonState(false, "Submit");
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", error, xhr.responseText);
            alert("Error: " + error);
            toggleButtonState(false, "Submit");
        }
    });
}

function toggleButtonState(disabled, text) {
    $("#submit")
        .prop("disabled", disabled)
        .css("pointer-events", disabled ? "none" : "auto")
        .text(text);

    $("#cancel").prop("disabled", disabled).css("pointer-events", disabled ? "none" : "auto");
}

function startCountdown(seconds) {
    let countdown = seconds;
    const button = $("#vieworder");

    function updateButton() {
        button.text(`View Order (${countdown}s)`);
        countdown--;

        if (countdown >= 0) {
            setTimeout(updateButton, 1000);
        } else {
            window.location.href = "/order/detail";
        }
    }
    updateButton();
}

function bindItemOrder(itemId) {
    $.ajax({
        type: "POST",
        url: "Method.aspx/Detail",
        data: JSON.stringify({ itemId }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const data = response.d;

            if (!data.length) return;

            const itemData = data[0];
            const blindId = itemData["BlindType"];
            const product = itemData["Product"];

            bindBlindType(designId);
            bindProduct(blindId);

            setTimeout(function () { setFormValues(itemData); }, 2500);
            setTimeout(function () {
                bindComponentForm(blindId, product);
            }, 2700);
        },
        error: function (xhr, status, error) {
            console.error("Error:", error);
        }
    });
}

function setFormValues(itemData) {
    const mapping = {
        blindtype: "BlindType",
        product: "Product",
        itemnumber: "AddNumber",
        cost: "Cost",
        itemname: "AddName",
        itemcode: "MicronetId",
        qty: "Qty"
    };

    Object.keys(mapping).forEach(id => {
        const el = document.getElementById(id);
        if (!el) {
            console.warn(`Elemen '${id}' tidak ditemukan.`);
            return;
        }
        let value = itemData[mapping[id]];
        el.value = value || "";
    });
}

function controlForm(status, isEditItem, isCopyItem) {
    if (isEditItem === undefined) {
        isEditItem = false;
    }
    if (isCopyItem === undefined) {
        isCopyItem = false;
    }

    document.getElementById("submit").style.display = status ? "none" : "";

    const inputs = ["blindtype", "product", "itemname", "itemcode", "qty", "itemnumber", "cost"];

    inputs.forEach(id => {
        const inputElement = document.getElementById(id);
        if (inputElement) {
            if (isCopyItem) {
                inputElement.disabled = (id === "blindtype");
            } else if (isEditItem && (id === "blindtype")) {
                inputElement.disabled = true;
            } else {
                inputElement.disabled = status;
            }
        }
    });
}