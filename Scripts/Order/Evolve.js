document.getElementById("modalSuccess").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalError").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

document.getElementById("modalInfo").addEventListener("hide.bs.modal", function () {
    document.activeElement.blur();
    document.body.focus();
});

$(document).ready(function () {
    checkSession();

    $("#submit").on("click", proccess);
    $("#cancel").on("click", () => window.location.href = "/order/detail/");
    $("#vieworder").on("click", () => window.location.href = "/order/detail");
    $("#reset").on("click", () => window.location.href = "/orders/evolve");

    $("#blindtype").on("change", function () {
        resetForm();

        const blindtype = $(this).val();
        const mounting = document.getElementById("mounting").value;
        const framebottom = document.getElementById("framebottom").value;
        const midrailheight1 = parseFloat(document.getElementById("midrailheight1").value) || 0;

        bindColourType(blindtype);
        bindMounting(blindtype);

        bindLayoutCode(blindtype);
        bindFrameType(blindtype, mounting);

        bindBottomTrack(blindtype, framebottom);

        bindTiltrodSplit(midrailheight1);

        visibleSemiInside(blindtype, mounting);
    });

    $("#colourtype").on("change", function () {
        const blindtype = document.getElementById("blindtype").value;
        bindComponentForm(blindtype, $(this).val());
    });

    $("#mounting").on("change", function () {
        const mounting = $(this).val();
        const blindtype = document.getElementById("blindtype").value;

        bindFrameType(blindtype, mounting);
        visibleSemiInside(blindtype, mounting);
    });

    $("#midrailheight1").on("input", function () {
        const midrailheight1 = parseFloat(document.getElementById("midrailheight1").value) || 0;
        const midrailheight2 = parseFloat(document.getElementById("midrailheight2").value) || 0;

        bindMidrailCritical(midrailheight1, midrailheight2);
        bindTiltrodSplit(midrailheight1);
    });

    $("#midrailheight2").on("input", function () {
        const midrailheight1 = parseFloat(document.getElementById("midrailheight1").value) || 0;
        const midrailheight2 = parseFloat(document.getElementById("midrailheight2").value) || 0;

        bindMidrailCritical(midrailheight1, midrailheight2);
    });

    $("#joinedpanels").on("change", function () {
        const blindtype = document.getElementById("blindtype").value;
        const hingecolour = document.getElementById("hingecolour").value;

        visibleHingeColour(blindtype, $(this).val());
        visibleHingesLoose(blindtype, hingecolour, $(this).val());
    });

    $("#hingecolour").on("change", function () {
        const blindtype = document.getElementById("blindtype").value;
        const joinedpanels = document.getElementById("joinedpanels").value;
        visibleHingesLoose(blindtype, $(this).val(), joinedpanels);
    });

    $("#layoutcode").on("change", function () {
        $("#layoutcodecustom").val("");
        $("#samesizepanel").val("");
        const blindtype = document.getElementById("blindtype").value;
        let layoutcode = $(this).val();
        visibleLayoutCustom(layoutcode);
        if (layoutcode === "Other") {
            layoutcode = document.getElementById("layoutcodecustom").value;
        }
        visibleSameSize(blindtype, layoutcode);
        visibleGap(blindtype, "", layoutcode);
    });

    $("#layoutcodecustom").on("input", function () {
        $("#samesizepanel").val("");
        const blindtype = document.getElementById("blindtype").value;
        const layoutcode = $(this).val();
        visibleSameSize(blindtype, layoutcode);
        visibleGap(blindtype, "", layoutcode);
    });

    $("#samesizepanel").on("change", function () {
        const blindtype = document.getElementById("blindtype").value;
        const layout = document.getElementById("layoutcode").value;
        const layoutcustom = document.getElementById("layoutcodecustom").value;

        let layoutcode = layout;
        if (layout === "Other") layoutcode = layoutcustom;
        visibleGap(blindtype, $(this).val(), layoutcode);
    });

    $("#frametype").on("change", function () {
        const blindtype = document.getElementById("blindtype").value;
        const frametype = $(this).val();
        const mounting = document.getElementById("mounting").value;

        bindLeftFrame(frametype);
        bindRightFrame(frametype);
        bindTopFrame(frametype, mounting);
        bindBottomFrame(frametype);
        visibleFrameDetail(frametype);
        visibleBuildout(blindtype, frametype);
    });

    $("#framebottom").on("change", function () {
        const blindtype = document.getElementById("blindtype").value;
        const framebottom = $(this).val();
        bindBottomTrack(blindtype, framebottom);
        visibleBottomTrack(blindtype, framebottom);
    });

    $("#horizontaltpostheight").on("input", function () {
        const value = parseFloat($(this).val()) || 0;
        const horizontalrequired = document.getElementById("divHorizontalTPostRequired");

        horizontalrequired.style.display = "none";
        if (value === 0) return;
        if (value > 0) horizontalrequired.style.display = "";
    });

    $("#tiltrodsplit").on("change", function () {
        visibleSplitHeight($(this).val());
    });

    $("#notes").on("input", function () {
        let maxLength = 1000;
        let currentLength = $(this).val().length;
        $("#notescount").text(`${currentLength}/${maxLength}`);
    });
});

async function checkSession() {
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

    try {
        await getDesignName(designId);
        await getDataHeader(headerId);
        await getFormAction(itemAction);
        await loader(itemAction);

        if (itemAction === "AddItem") {
            bindComponentForm("", "");
            controlForm(false);
            await bindBlindType(designId);
        } else if (["EditItem", "ViewItem", "CopyItem"].includes(itemAction)) {
            await bindItemOrder(itemId);
            controlForm(
                itemAction === "ViewItem",
                itemAction === "EditItem",
                itemAction === "CopyItem"
            );
        }
    } catch (error) {
        reject(error);
    }
}

function isError(msg) {
    $("#modalError").modal("show");
    document.getElementById("errorMsg").innerHTML = msg;
}

function loader(itemAction) {
    return new Promise((resolve) => {
        if (itemAction === "AddItem") {
            document.getElementById("divLoader").style.display = "none";
            document.getElementById("divOrder").style.display = "";
        }
        resolve();
    });
}

function getDataHeader(headerId) {
    return new Promise((resolve, reject) => {
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
                    document.getElementById("orderno").innerText =
                        data.OrderNumber || "-";
                    document.getElementById("ordername").innerText =
                        data.OrderName || "-";
                }
                resolve();
            },
            error: function (xhr, status, error) {
                reject(error);
            },
        });
    });
}

function getFormAction(itemAction) {
    return new Promise((resolve) => {
        const cardTitle = document.getElementById("cardTitle");
        if (!cardTitle) {
            resolve();
            return;
        }

        const actionMap = {
            AddItem: "ADD ITEM",
            EditItem: "EDIT ITEM",
            ViewItem: "VIEW ITEM",
            CopyItem: "COPY ITEM",
        };
        cardTitle.innerText = actionMap[itemAction] || "";
        resolve();
    });
}

function getDesignName(designId) {
    return new Promise((resolve, reject) => {
        const pageTitle = document.getElementById("pageTitle");
        pageTitle.textContent = "";

        if (!designId) {
            resolve();
            return;
        }

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
                resolve();
            },
            error: function (error) {
                reject(error);
            },
        });
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
            error: function (error) {
                reject(error);
            }
        });
    });
}

function bindBlindType(designId) {
    return new Promise((resolve, reject) => {
        const blindtype = document.getElementById("blindtype");
        blindtype.innerHTML = "";

        if (!designId) {
            resolve();
            return;
        }

        const listData = { type: "BlindTypeShutters", designtype: designId };

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
                        bindColourType(selectedValue),
                        bindMounting(selectedValue),
                    ])
                        .then(resolve)
                        .catch(reject);
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            },
        });
    });
}

function bindColourType(blindType) {
    return new Promise((resolve, reject) => {
        const colourtype = document.getElementById("colourtype");
        colourtype.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        const listData = {
            type: "ColourType",
            blindtype: blindType,
            tubetype: "N/A",
            controltype: "N/A",
        };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    colourtype.innerHTML = "";
                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        colourtype.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        colourtype.add(option);
                    });

                    let selectedValue = "";

                    if (response.d.length === 1) {
                        colourtype.selectedIndex = 0;
                        selectedValue = colourtype.value;
                    }

                    Promise.resolve(bindComponentForm(blindType, selectedValue))
                        .then(resolve)
                        .catch(reject);
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            },
        });
    });
}

function bindMounting(blindId) {
    return new Promise((resolve, reject) => {
        const mounting = document.getElementById("mounting");

        if (!blindId) {
            resolve();
            return;
        }

        const listData = { type: "Mounting", blindtype: blindId };

        $.ajax({
            type: "POST",
            url: "Method.aspx/ListData",
            data: JSON.stringify({ data: listData }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (Array.isArray(response.d)) {
                    mounting.innerHTML = "";

                    if (response.d.length > 1) {
                        const defaultOption = document.createElement("option");
                        defaultOption.text = "";
                        defaultOption.value = "";
                        mounting.add(defaultOption);
                    }

                    response.d.forEach(function (item) {
                        const option = document.createElement("option");
                        option.value = item.Value;
                        option.text = item.Text;
                        mounting.add(option);
                    });

                    if (response.d.length === 1) {
                        mounting.selectedIndex = 0;
                    }
                }
                resolve();
            },
            error: function (error) {
                reject(error);
            },
        });
    });
}

function bindMidrailCritical(height1, height2) {
    return new Promise((resolve) => {
        const midrailcritical = document.getElementById("midrailcritical");
        midrailcritical.innerHTML = "";

        visibleMidrail(height1);

        let options = [{ value: "", text: "" }];

        if (height1 > 0 && height2 > 0) {
            options = [
                { value: "", text: "" },
                { value: "Yes - Top Only", text: "YES - TOP ONLY" },
                { value: "Yes - Bottom Only", text: "YES - BOTTOM ONLY" },
            ];
        } else if (height1 > 0) {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
            ];
        } else if (height2 > 0) {
            options = [
                { value: "", text: "" },
                { value: "Yes - Top Only", text: "YES - TOP ONLY" },
                { value: "Yes - Bottom Only", text: "YES - BOTTOM ONLY" },
            ];
        }

        options.forEach((opt) => {
            const optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            midrailcritical.appendChild(optionElement);
        });

        resolve();
    });
}

function bindLayoutCode(blindType) {
    return new Promise((resolve, reject) => {
        const layoutcode = document.getElementById("layoutcode");
        layoutcode.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        getBlindName(blindType)
            .then((blindName) => {
                let options = [{ value: "", text: "" }];

                switch (blindName) {
                    case "Hinged":
                        options = [
                            { value: "", text: "" },
                            { value: "L", text: "L" },
                            { value: "R", text: "R" },
                            { value: "LR", text: "LR" },
                            { value: "LD-R", text: "LD-R" },
                            { value: "L-DR", text: "L-DR" },
                            { value: "LTLR", text: "LTLR" },
                            { value: "LRTR", text: "LRTR" },
                            { value: "LRTLR", text: "LRTLR" },
                            { value: "LTLRTR", text: "LTLRTR" },
                            { value: "LD-RTLD-R", text: "LD-RTLD-R" },
                            { value: "L-DRTL-DR", text: "L-DRTL-DR" },
                            { value: "Other", text: "OTHER" },
                        ];
                        break;
                    case "Hinged Bi-fold":
                        options = [
                            { value: "", text: "" },
                            { value: "LL", text: "LL" },
                            { value: "RR", text: "RR" },
                            { value: "LLRR", text: "LLRR" },
                            { value: "Other", text: "OTHER" },
                        ];
                        break;
                    case "Track Bi-fold":
                        options = [
                            { value: "", text: "" },
                            { value: "LL", text: "LL" },
                            { value: "RR", text: "RR" },
                            { value: "LLRR", text: "LLRR" },
                            { value: "LLLL", text: "LLLL" },
                            { value: "RRRR", text: "RRRR" },
                            { value: "LLRRRR", text: "LLRRRR" },
                            { value: "LLLLRR", text: "LLLLRR" },
                            { value: "LLLLLL", text: "LLLLLL" },
                            { value: "RRRRRR", text: "RRRRRR" },
                            { value: "LLRRRRRR", text: "LLRRRRRR" },
                            { value: "LLLLRRRR", text: "LLLLRRRR" },
                            { value: "LLLLLLRR", text: "LLLLLLRR" },
                            { value: "LLLLLLLL", text: "LLLLLLLL" },
                            { value: "RRRRRRRR", text: "RRRRRRRR" },
                            { value: "Other", text: "OTHER" },
                        ];
                        break;
                    case "Track Sliding":
                        options = [
                            { value: "", text: "" },
                            { value: "BF", text: "BF" },
                            { value: "FB", text: "FB" },
                            { value: "BFB", text: "BFB" },
                            { value: "FBF", text: "FBF" },
                            { value: "BFFB", text: "BFFB" },
                            { value: "FBBF", text: "FBBF" },
                            { value: "BBFF", text: "BBFF" },
                            { value: "FFBB", text: "FFBB" },
                            { value: "Other", text: "OTHER" },
                        ];
                        break;
                    case "Track Sliding Single Track":
                        options = [
                            { value: "", text: "" },
                            { value: "F", text: "F" },
                            { value: "FF", text: "FF" },
                            { value: "FFF", text: "FFF" },
                            { value: "FFFF", text: "FFFF" },
                            { value: "Other", text: "OTHER" },
                        ];
                        break;
                    case "Fixed":
                        options = [
                            { value: "", text: "" },
                            { value: "F", text: "F" },
                            { value: "FF", text: "FF" },
                            { value: "FFF", text: "FFF" },
                            { value: "FFFF", text: "FFFF" },
                            { value: "FFFFFF", text: "FFFFFF" },
                            { value: "Other", text: "OTHER" },
                        ];
                        break;
                }

                options.forEach((opt) => {
                    let optionElement = document.createElement("option");
                    optionElement.value = opt.value;
                    optionElement.textContent = opt.text;
                    layoutcode.appendChild(optionElement);
                });

                resolve();
            })
            .catch((error) => {
                reject(error);
            });
    });
}

function bindFrameType(blindType, mounting) {
    return new Promise((resolve, reject) => {
        const frametype = document.getElementById("frametype");
        frametype.innerHTML = "";

        visibleFrameDetail(frametype.value);
        visibleBuildout(blindType, frametype.value);

        if (!blindType) {
            resolve();
            return;
        }

        getBlindName(blindType).then(blindName => {
            let options = [{ value: "", text: "" }];

            if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
                options = [
                    { value: "", text: "" },
                    { value: "Beaded L 49mm", text: "BEADED L 49MM" },
                    { value: "Insert L 49mm", text: "INSERT L 49MM" },
                    { value: "No Frame", text: "NO FRAME" }
                ];
                if (mounting === "Inside") {
                    options = [
                        { value: "", text: "" },
                        { value: "Beaded L 49mm", text: "BEADED L 49MM" },
                        { value: "Insert L 49mm", text: "INSERT L 49MM" },
                        { value: "Small Bullnose Z Frame", text: "SMALL BULLNOSE Z FRAME" },
                        { value: "Large Bullnose Z Frame", text: "LARGE BULLNOSE Z FRAME" },
                        { value: "No Frame", text: "NO FRAME" }
                    ];
                }
            } else if (blindName === "Track Bi-fold") {
                options = [
                    { value: "", text: "" },
                    { value: "92mm", text: "92MM" },
                    { value: "152mm", text: "152MM" },
                    { value: "185mm", text: "185MM" }
                ];
            } else if (blindName === "Track Sliding") {
                options = [
                    { value: "", text: "" },
                    { value: "152mm", text: "152MM" },
                    { value: "185mm", text: "185MM" }
                ];
            } else if (blindName === "Track Sliding Single Track") {
                options = [
                    { value: "", text: "" },
                    { value: "92mm", text: "92MM" },
                    { value: "152mm", text: "152MM" },
                    { value: "185mm", text: "185MM" }
                ];
            } else if (blindName === "Fixed") {
                options = [
                    { value: "", text: "" },
                    { value: "U Channel", text: "U CHANNEL" },
                    { value: "19x19 Light Block", text: "19X19 LIGHT BLOCK" }
                ];
            }
            options.forEach(opt => {
                let optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                frametype.appendChild(optionElement);
            });

            if (frametype.options.length === 1) {
                bindLeftFrame(frametype.value);
                bindRightFrame(frametype.value);
                bindTopFrame(frametype.value, mounting);
                bindBottomFrame(frametype.value);
            }

            resolve();
        }).catch(error => {
            reject(error);
        });
    });
}

function bindLeftFrame(frameType) {
    return new Promise((resolve, reject) => {
        const frameleft = document.getElementById("frameleft");
        frameleft.innerHTML = "";

        if (!frameType) {
            resolve();
            return;
        }

        let options = [{ value: "", text: "" }];

        if (frameType === "Beaded L 49mm" || frameType === "Insert L 49mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" }
            ];
        } else if (frameType === "Small Bullnose Z Frame" || frameType === "Large Bullnose Z Frame") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" },
                //{ value: "Bullnose Z Sill Plate", text: "BULLNOSE Z SILL PLATE" },
                { value: "Sill Plate (Bullnose Z)", text: "SILL PLATE (BULLNOSE Z)" },
            ];
        } else if (frameType === "No Frame") {
            options = [{ value: "Light Block", text: "LIGHT BLOCK" }];
        } else if (frameType === "92mm" || frameType === "152mm" || frameType === "185mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" }
            ];
        } else if (frameType === "U Channel") {
            options = [{ value: "No", text: "NO" }];
        } else if(frameType === "19x19 Light Block"){
            options = [{ value: "No", text: "NO" }];
        }
        options.forEach(opt => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            frameleft.appendChild(optionElement);
        });

        resolve();
    });
}

function bindRightFrame(frameType) {
    return new Promise((resolve, reject) => {
        const frameright = document.getElementById("frameright");
        frameright.innerHTML = "";

        if (!frameType) {
            resolve();
            return;
        }

        let options = [{ value: "", text: "" }];

        if (frameType === "Beaded L 49mm" || frameType === "Insert L 49mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" }
            ];
        } else if (frameType === "Small Bullnose Z Frame" || frameType === "Large Bullnose Z Frame") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" },
                //{ value: "Bullnose Z Sill Plate", text: "BULLNOSE Z SILL PLATE" },
                { value: "Sill Plate (Bullnose Z)", text: "SILL PLATE (BULLNOSE Z)" },
            ];
        } else if (frameType === "No Frame") {
            options = [{ value: "Light Block", text: "LIGHT BLOCK" }];
        } else if (frameType === "92mm" || frameType === "152mm" || frameType === "185mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" }
            ];
        } else if (frameType === "U Channel") {
            options = [{ value: "No", text: "NO" }];
        } else if(frameType === "19x19 Light Block"){
            options = [{ value: "No", text: "NO" }];
        }
        options.forEach(opt => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            frameright.appendChild(optionElement);
        });

        resolve();
    });
}

function bindTopFrame(frameType, mounting) {
    return new Promise((resolve, reject) => {
        const frametop = document.getElementById("frametop");
        frametop.innerHTML = "";

        if (!frameType) {
            resolve();
            return;
        }

        let options = [{ value: "", text: "" }];

        if (frameType === "Beaded L 49mm" || frameType === "Insert L 49mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" }
            ];
        } else if (frameType === "Small Bullnose Z Frame" || frameType === "Large Bullnose Z Frame") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" },
                //{ value: "Bullnose Z Sill Plate", text: "BULLNOSE Z SILL PLATE" },
                { value: "Sill Plate (Bullnose Z)", text: "SILL PLATE (BULLNOSE Z)" },
                { value: "Roller Catch Ramp", text: "ROLLER CATCH RAMP" }
            ];
        } else if (frameType === "No Frame") {
            options = [{ value: "Light Block", text: "LIGHT BLOCK" }];
        } else if (frameType === "92mm" || frameType === "152mm" || frameType === "185mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" }
            ];
            if (mounting === "Inside") {
                options = [{ value: "Yes", text: "YES" }];
            }
        } else if (frameType === "U Channel") {
            options = [
                { value: "", text: "" },
                { value: "No", text: "NO" },
                { value: "U Channel", text: "U CHANNEL" }
            ];
        } else if(frameType === "19x19 Light Block"){
            options = [{ value: "No", text: "NO" }];
        }
        options.forEach(opt => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            frametop.appendChild(optionElement);
        });
        resolve();
    });
}

function bindBottomFrame(frameType) {
    return new Promise((resolve, reject) => {
        const framebottom = document.getElementById("framebottom");
        framebottom.innerHTML = "";

        bindBottomTrack("", "");

        if (!frameType) {
            resolve();
            return;
        }

        let options = [{ value: "", text: "" }];

        if (frameType === "Beaded L 49mm" || frameType === "Insert L 49mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" }
            ];
        } else if (frameType === "Small Bullnose Z Frame" || frameType === "Large Bullnose Z Frame") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" },
                { value: "Light Block", text: "LIGHT BLOCK" },
                //{ value: "Bullnose Z Sill Plate", text: "BULLNOSE Z SILL PLATE" },
                { value: "Sill Plate (Bullnose Z)", text: "SILL PLATE (BULLNOSE Z)" },
                { value: "Roller Catch Ramp", text: "ROLLER CATCH RAMP" }
            ];
        } else if (frameType === "No Frame") {
            options = [{ value: "Light Block", text: "LIGHT BLOCK" }];
        } else if (frameType === "92mm" || frameType === "152mm" || frameType === "185mm") {
            options = [
                { value: "", text: "" },
                { value: "Yes", text: "YES" },
                { value: "No", text: "NO" }
            ];
        } else if (frameType === "U Channel") {
            options = [
                { value: "", text: "" },
                { value: "No", text: "NO" },
                { value: "U Channel", text: "U CHANNEL" }
            ];
        } else if(frameType === "19x19 Light Block"){
            options = [{ value: "No", text: "NO" }];
        }
        options.forEach(opt => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            framebottom.appendChild(optionElement);
        });

        resolve();
    });
}

function bindBottomTrack(blindType, frameBottom) {
    return new Promise((resolve, reject) => {
        const bottomtracktype = document.getElementById("bottomtracktype");
        bottomtracktype.innerHTML = "";

        if (!blindType) {
            resolve();
            return;
        }

        getBlindName(blindType).then(blindName => {
            let options = [{ value: "", text: "" }];

            if (blindName === "Track Bi-fold" || blindName === "Track Sliding" || blindName === "Track Sliding Single Track") {
                options = [
                    { value: "", text: "" },
                    { value: "M Track", text: "M TRACK" }
                ];
            }
            options.forEach(opt => {
                let optionElement = document.createElement("option");
                optionElement.value = opt.value;
                optionElement.textContent = opt.text;
                bottomtracktype.appendChild(optionElement);
            });
            resolve();
        }).catch(error => {
            reject(error);
        });
    });
}

function bindTiltrodSplit(height1) {
    return new Promise((resolve, reject) => {
        const tiltrodsplit = document.getElementById("tiltrodsplit");
        tiltrodsplit.innerHTML = "";

        visibleSplitHeight(tiltrodsplit.value);

        const thisValue = parseFloat(height1) || 0;

        let options = [{ value: "", text: "" }];

        if (thisValue === 0) {
            options = [
                { value: "", text: "" },
                { value: "None", text: "NONE" },
                { value: "Split Halfway", text: "SPLIT HALFWAY" },
                { value: "Other", text: "OTHER" },
            ];
        } else if (thisValue > 0) {
            options = [
                { value: "", text: "" },
                { value: "None", text: "NONE" },
                { value: "Split Halfway Above Midrail", text: "SPLIT HALFWAY ABOVE MIDRAIL" },
                { value: "Split Halfway Below Midrail", text: "SPLIT HALFWAY BELOW MIDRAIL" },
                { value: "Split Halfway Above and Below Midrail", text: "SPLIT HALFWAY ABOVE & BELOW MIDRAIL" },
                { value: "Other", text: "OTHER" },
            ];
        }

        options.forEach(opt => {
            let optionElement = document.createElement("option");
            optionElement.value = opt.value;
            optionElement.textContent = opt.text;
            tiltrodsplit.appendChild(optionElement);
        });

        if (tiltrodsplit.options.length === 0) {
            tiltrodsplit.selectedIndex = 0;
            visibleSplitHeight(tiltrodsplit.value);
        }

        resolve();
    });
}

function bindComponentForm(blindType, colourType) {
    const divDetail = document.getElementById("divDetail");
    divDetail.style.display = colourType ? "" : "none";
    if (!colourType) return;

    const elements = [
        "divSemiInsideMount", "divLouvrePosition", "divMidrailHeight2", "divMidrailCritical", "divPanelQty", "divJoinedPanels",
        "divHingeColour", "divCustomHeaderLength", "divLayoutCode", "divLayoutCodeCustom", "divFrameType",
        "divFrameLeft", "divFrameRight", "divFrameTop", "divFrameBottom", "divBottomTrackType",
        "divBuildout", "divSameSize", "divGapPost", "divHorizontalTPost", "divHorizontalTPostRequired",
        "divTiltrodType", "divTiltrodSplit", "divTiltrodHeight", "divReverseHinged", "divPelmetFlat",
        "divExtraFascia", "divHingesLoose"
    ];

    elements.forEach(id => {
        const el = document.getElementById(id);
        if (el) el.style.display = "none";
    });

    getBlindName(blindType).then(blindName => {
        if (blindName === "Panel Only") {
            ["divPanelQty", "divTiltrodType", "divTiltrodSplit"].forEach(id => document.getElementById(id).style.display = "");
        } else if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
            ["divHingeColour", "divTiltrodType", "divTiltrodSplit", "divLayoutCode", "divFrameType", "divHorizontalTPost"].forEach(id => document.getElementById(id).style.display = "");
        } else if (blindName === "Track Bi-fold") {
            ["divHingeColour", "divTiltrodType", "divTiltrodSplit", "divLayoutCode", "divFrameType", "divReverseHinged", "divExtraFascia"].forEach(id => document.getElementById(id).style.display = "");
        } else if (blindName === "Track Sliding") {
            ["divLouvrePosition", "divJoinedPanels", "divCustomHeaderLength", "divLayoutCode", "divFrameType", "divFrameLeft", "divFrameRight", "divFrameTop", "divFrameBottom", "divTiltrodType", "divTiltrodSplit", "divExtraFascia"].forEach(id => document.getElementById(id).style.display = "");
        } else if (blindName === "Track Sliding Single Track") {
            ["divJoinedPanels", "divCustomHeaderLength", "divLayoutCode", "divFrameType", "divFrameLeft", "divFrameRight", "divFrameTop", "divFrameBottom", "divTiltrodType", "divTiltrodSplit", "divExtraFascia"].forEach(id => document.getElementById(id).style.display = "");
        } else if (blindName === "Fixed") {
            ["divLayoutCode", "divFrameType", "divTiltrodType", "divTiltrodSplit"].forEach(id => document.getElementById(id).style.display = "");
            console.log("No additional elements to show for Fixed type.");
        }
    }).catch(error => {
        console.error("Gagal mendapatkan blind name:", error);
    });
}

function visibleMidrail(height1) {
    return new Promise((resolve) => {
        const divMidrailHeight2 = document.getElementById("divMidrailHeight2");
        const divMidrailCritical = document.getElementById("divMidrailCritical");

        divMidrailHeight2.style.display = "none";
        divMidrailCritical.style.display = "none";

        if (height1 > 0) {
            divMidrailHeight2.style.display = "";
            divMidrailCritical.style.display = "";
        }

        resolve();
    });
}

function visibleHingeColour(blindType, joinedPanels) {
    return new Promise((resolve, reject) => {
        const divHingeColour = document.getElementById("divHingeColour");
        divHingeColour.style.display = "none";

        if (!blindType) return resolve();

        getBlindName(blindType)
            .then((blindName) => {
                if (
                    blindName === "Hinged" ||
                    blindName === "Hinged Bi-fold" ||
                    blindName === "Track Bi-fold"
                ) {
                    divHingeColour.style.display = "";
                } else if (
                    blindName === "Track Sliding" ||
                    blindName === "Track Sliding Single Track"
                ) {
                    if (joinedPanels === "Yes") {
                        divHingeColour.style.display = "";
                    }
                }

                resolve();
            })
            .catch((error) => {
                reject(error);
            });
    });
}

function visibleLayoutCustom(layout) {
    return new Promise((resolve) => {
        const divLayoutCodeCustom = document.getElementById("divLayoutCodeCustom");
        divLayoutCodeCustom.style.display = "none";
        if (layout === "Other") {
            divLayoutCodeCustom.style.display = "";
        }

        resolve();
    });
}

function visibleFrameDetail(frameType) {
    return new Promise((resolve) => {
        const frameElements = [
            document.getElementById("divFrameLeft"),
            document.getElementById("divFrameRight"),
            document.getElementById("divFrameTop"),
            document.getElementById("divFrameBottom"),
        ];

        const shouldHide = frameType === "" || frameType === "19x19 Light Block";
        const displayValue = shouldHide ? "none" : "";

        frameElements.forEach((element) => {
            element.style.display = displayValue;
        });

        resolve();
    });
}

function visibleBuildout(blindType, frameType) {
    return new Promise((resolve, reject) => {
        const divBuildout = document.getElementById("divBuildout");
        divBuildout.style.display = "none";

        if (!blindType || !frameType) return resolve();

        getBlindName(blindType)
            .then((blindName) => {
                if ((blindName === "Hinged" || blindName === "Hinged Bi-fold") && frameType === "Insert L 49mm") {
                    divBuildout.style.display = "";
                }
                resolve();
            })
            .catch((error) => {
                reject(error);
            });
    });
}

function visibleHingesLoose(blindType, hingeColour, joinedPanels) {
    return new Promise((resolve, reject) => {
        const divHingesLoose = document.getElementById("divHingesLoose");
        divHingesLoose.style.display = "none";

        if (!hingeColour) return resolve();

        getBlindName(blindType)
            .then((blindName) => {
                if (
                    blindName === "Hinged" ||
                    blindName === "Hinged Bi-fold" ||
                    blindName === "Track Bi-fold"
                ) {
                    divHingesLoose.style.display = "";
                } else if (blindName === "Track Sliding" && joinedPanels === "Yes") {
                    divHingesLoose.style.display = "";
                }
                resolve();
            })
            .catch((error) => {
                reject(error);
            });
    });
}

function visibleSemiInside(blindType, mounting) {
    return new Promise((resolve, reject) => {
        const divSemiInside = document.getElementById("divSemiInsideMount");
        divSemiInside.style.display = "none";

        if (!blindType) return resolve();

        getBlindName(blindType)
            .then((blindName) => {
                if (
                    (blindName === "Track Bi-fold" ||
                        blindName === "Track Sliding" ||
                        blindName === "Track Sliding Single Track") &&
                    mounting === "Inside"
                ) {
                    divSemiInside.style.display = "";
                }
                resolve();
            })
            .catch((error) => {
                console.error("Gagal mendapatkan blind name:", error);
                reject(error);
            });
    });
}

function visibleSplitHeight(tiltrodSplit) {
    return new Promise((resolve) => {
        const tiltrodHeight = document.getElementById("divTiltrodHeight");
        tiltrodHeight.style.display = "none";

        if (tiltrodSplit === "Other") tiltrodHeight.style.display = "";

        resolve();
    });
}

function visibleSameSize(blindType, layoutCode) {
    return new Promise((resolve, reject) => {
        const divSameSize = document.getElementById("divSameSize");
        divSameSize.style.display = "none";

        if (!blindType && !layoutCode) return resolve();

        getBlindName(blindType)
            .then((blindName) => {
                if (
                    (blindName === "Hinged" || blindName === "Hinged Bi-fold") &&
                    cekSameSizePanels(layoutCode)
                ) {
                    divSameSize.style.display = "";
                }
                resolve();
            })
            .catch((error) => {
                reject(error);
            });
    });
}

function visibleGap(blindType, sameSize, layoutCode) {
    return new Promise((resolve, reject) => {
        const divGapPost = document.getElementById("divGapPost");
        divGapPost.style.display = "none";

        const divGap1 = document.getElementById("divGap1");
        const divGap2 = document.getElementById("divGap2");
        const divGap3 = document.getElementById("divGap3");
        const divGap4 = document.getElementById("divGap4");
        const divGap5 = document.getElementById("divGap5");

        divGap1.style.display = "none";
        divGap2.style.display = "none";
        divGap3.style.display = "none";
        divGap4.style.display = "none";
        divGap5.style.display = "none";

        if (!blindType) return resolve();

        getBlindName(blindType)
            .then((blindName) => {
                if (blindName === "Hinged" || blindName === "Hinged Bi-fold") {
                    if (cekSameSizePanels(layoutCode)) {
                        if (sameSize === "Yes") {
                            divGapPost.style.display = "none";
                        } else {
                            let countT = 0;
                            for (let char of layoutCode) {
                                if (char === "T" || char === "B" || char === "C" || char === "G")
                                    countT++;
                            }
                            if (countT > 0) divGapPost.style.display = "";
                            if (countT > 0) divGap1.style.display = "";
                            if (countT > 1) divGap2.style.display = "";
                            if (countT > 2) divGap3.style.display = "";
                            if (countT > 3) divGap4.style.display = "";
                            if (countT > 4) divGap5.style.display = "";
                        }
                    } else {
                        let countT = 0;
                        for (let char of layoutCode) {
                            if (char === "T" || char === "B" || char === "C" || char === "G")
                                countT++;
                        }
                        if (countT > 0) divGapPost.style.display = "";
                        if (countT > 0) divGap1.style.display = "";
                        if (countT > 1) divGap2.style.display = "";
                        if (countT > 2) divGap3.style.display = "";
                        if (countT > 3) divGap4.style.display = "";
                        if (countT > 4) divGap5.style.display = "";
                    }
                } else if (blindName === "Track Bi-fold") {
                    let countT = 0;
                    for (let char of layoutCode) {
                        if (char === "T" || char === "B" || char === "C" || char === "G")
                            countT++;
                    }
                    if (countT > 0) divGapPost.style.display = "";
                    if (countT > 0) divGap1.style.display = "";
                    if (countT > 1) divGap2.style.display = "";
                    if (countT > 2) divGap3.style.display = "";
                    if (countT > 3) divGap4.style.display = "";
                    if (countT > 4) divGap5.style.display = "";
                }
                resolve();
            })
            .catch((error) => {
                reject(error);
            });
    });
}

function visibleBottomTrack(blindType, frameBottom) {
    return new Promise((resolve, reject) => {
        const divBottomTrackType = document.getElementById("divBottomTrackType");
        divBottomTrackType.style.display = "none";

        if (!blindType) return resolve();

        getBlindName(blindType).then((blindName) => {
            if (blindName === "Track Bi-fold" || blindName === "Track Sliding" || blindName === "Track Sliding Single Track") {
                if (frameBottom === "No") {
                    divBottomTrackType.style.display = "";
                }
            }
            resolve();
        }).catch((error) => {
            reject(error);
        });
    });
}

function proccess() {
    toggleButtonState(true, "Processing...");

    const fields = [
        "blindtype", "colourtype", "qty", "room", "mounting", "width", "drop", "semiinsidemount",
        "louvresize", "louvreposition", "midrailheight1", "midrailheight2", "midrailcritical",
        "panelqty", "joinedpanels", "hingecolour", "customheaderlength", "layoutcode", "layoutcodecustom",
        "frametype", "frameleft", "frameright", "frametop", "framebottom", "bottomtracktype", "buildout",
        "samesizepanel", "gap1", "gap2", "gap3", "gap4", "gap5", "horizontaltpostheight", "horizontaltpost",
        "tiltrodtype", "tiltrodsplit", "splitheight1", "splitheight2",
        "reversehinged", "pelmetflat", "extrafascia", "hingesloose", "markup", "notes"
    ];

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
        url: "Method.aspx/EvolveProccess",
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
        error: function () {
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
    document.getElementById("divLoader").style.display = "";

    return new Promise((resolve, reject) => {
        $.ajax({
            type: "POST",
            url: "Method.aspx/ShutterDetail",
            data: JSON.stringify({ itemId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                const data = response.d;

                if (!data.length) {
                    document.getElementById("divLoader").style.display = "none";
                    return;
                }

                const itemData = data[0];
                const {
                    BlindType: blindtype,
                    ColourType: colourtype,
                    Mounting: mounting,
                    MidrailHeight1: height1,
                    MidrailHeight2: height2,
                    Buildout: buildout,
                    FrameType: frameType,
                    FrameBottom: bottomFrame,
                    JoinedPanels: joinedPanels,
                    LayoutCode: layoutCode,
                    LayoutCodeCustom: layoutCodeCustom,
                    SamePanelSize: sameSize,
                    HingeColour: hingeColour,
                    TiltrodSplit: tiltrodSplit,
                } = itemData;

                let layoutCodeFinal =
                    layoutCode === "Other" ? layoutCodeCustom : layoutCode;

                Promise.resolve()
                    .then(() => bindBlindType(designId))
                    .then(() => bindColourType(blindtype))
                    .then(() => bindMounting(blindtype))
                    .then(() => bindMidrailCritical(height1, height2))
                    .then(() => bindLayoutCode(blindtype))
                    .then(() =>
                        bindFrameType(blindtype, mounting)
                    )
                    .then(() => bindLeftFrame(frameType))
                    .then(() => bindRightFrame(frameType))
                    .then(() => bindTopFrame(frameType, mounting))
                    .then(() => bindBottomFrame(frameType))
                    .then(() => bindBottomTrack(blindtype, bottomFrame))
                    .then(() => bindTiltrodSplit(height1))
                    .then(() => setFormValues(itemData))
                    .then(() => bindComponentForm(blindtype, colourtype))
                    .then(() => {
                        return Promise.all([
                            visibleMidrail(height1),
                            visibleSemiInside(blindtype, mounting),
                            visibleHingeColour(blindtype, joinedPanels),
                            visibleLayoutCustom(layoutCode),
                            visibleFrameDetail(frameType),
                            visibleBottomTrack(blindtype, bottomFrame),
                            visibleSameSize(blindtype, layoutCodeFinal),
                            visibleGap(blindtype, sameSize, layoutCodeFinal),
                            visibleHingesLoose(blindtype, hingeColour, joinedPanels),
                            visibleSplitHeight(tiltrodSplit)
                        ]);
                    })
                    .then(() => {
                        document.getElementById("divLoader").style.display = "none";
                        document.getElementById("divOrder").style.display = "";
                        resolve();
                    })
                    .catch((error) => {
                        document.getElementById("divLoader").style.display = "none";
                        reject(error);
                    });
            },
            error: function (error) {
                document.getElementById("divLoader").style.display = "none";
                reject(error);
            },
        });
    });
}

function setFormValues(itemData) {
    const mapping = {
        blindtype: "BlindType",
        colourtype: "ColourType",
        qty: "Qty",
        room: "Room",
        mounting: "Mounting",
        width: "Width",
        drop: "Drop",
        louvresize: "LouvreSize",
        midrailheight1: "MidrailHeight1",
        midrailheight2: "MidrailHeight2",
        midrailcritical: "MidrailCritical",
        semiinsidemount: "SemiInsideMount",
        panelqty: "PanelQty",
        joinedpanels: "JoinedPanels",
        hingecolour: "HingeColour",
        customheaderlength: "CustomHeaderLength",
        layoutcode: "LayoutCode",
        layoutcodecustom: "LayoutCodeCustom",
        frametype: "FrameType",
        frameleft: "FrameLeft",
        frameright: "FrameRight",
        frametop: "FrameTop",
        framebottom: "FrameBottom",
        bottomtracktype: "BottomTrackType",
        buildout: "Buildout",
        samesizepanel: "SamePanelSize",
        gap1: "Gap1",
        gap2: "Gap2",
        gap3: "Gap3",
        gap4: "Gap4",
        gap5: "Gap5",
        horizontaltpostheight: "HorizontalTPostHeight",
        horizontaltpost: "HorizontalTPost",
        tiltrodtype: "TiltrodType",
        tiltrodsplit: "TiltrodSplit",
        splitheight1: "SplitHeight1",
        splitheight2: "SplitHeight2",
        reversehinged: "ReverseHinged",
        pelmetflat: "PelmetFlat",
        extrafascia: "ExtraFascia",
        hingesloose: "HingesLoose",
        notes: "Notes",
        markup: "MarkUp"
    };

    Object.keys(mapping).forEach(id => {
        const el = document.getElementById(id);
        if (!el) {
            return;
        }

        let value = itemData[mapping[id]];
        if (id === "markup" && value === 0) value = "";
        el.value = value || "";
    });
    const maxLength = 1000;
    const notesLength = (itemData["Notes"] || "").length;
    $("#notescount").text(`${notesLength}/${maxLength}`);

    if (itemAction === "CopyItem") {
        const resetFields = ["room", "width", "drop", "notes"];
        resetFields.forEach(id => {
            const el = document.getElementById(id);
            if (el) el.value = "";
        });

        $("#notescount").text(`0/${maxLength}`);
    }
}

function controlForm(status, isEditItem, isCopyItem) {
    if (isEditItem === undefined) {
        isEditItem = false;
    }
    if (isCopyItem === undefined) {
        isCopyItem = false;
    }

    const submit = document.getElementById("submit");
    if (status === true) {
        submit.style.display = "none";
    } else {
        submit.style.display = "";
    }

    const inputs = [
        "blindtype", "colourtype", "qty", "room", "mounting", "width", "drop", "louvresize", "semiinsidemount",
        "midrailheight1", "midrailheight2", "midrailcritical", "panelqty", "joinedpanels", "hingecolour", "customheaderlength", "layoutcode", "layoutcodecustom", "frametype", "frameleft",
        "frameright", "frametop", "framebottom", "bottomtracktype", "buildout", "samesizepanel", "gap1", "gap2", "gap3", "gap4", "gap5", "horizontaltpostheight",
        "horizontaltpost", "tiltrodtype", "tiltrodsplit", "splitheight1", "splitheight2", "reversehinged",
        "pelmetflat", "extrafascia", "hingesloose", "notes", "markup"
    ];

    inputs.forEach(id => {
        const inputElement = document.getElementById(id);
        if (inputElement) {
            if (isCopyItem) {
                inputElement.disabled = (id === "blindtype");
            } else if (isEditItem && (id === "qty" || id === "blindtype")) {
                inputElement.disabled = true;
            } else {
                inputElement.disabled = status;
            }
        }
    });
}

function cekSameSizePanels(layoutCode) {
    if (layoutCode.length === 0) return false;
    if (layoutCode.includes("T")) {
        if (
            layoutCode.includes("B") ||
            layoutCode.includes("C") ||
            layoutCode.includes("G")
        ) {
            return false;
        }
    }
    if (layoutCode.includes("B")) {
        if (
            layoutCode.includes("T") ||
            layoutCode.includes("C") ||
            layoutCode.includes("G")
        ) {
            return false;
        }
    }
    if (layoutCode.includes("C")) {
        if (
            layoutCode.includes("T") ||
            layoutCode.includes("B") ||
            layoutCode.includes("G")
        ) {
            return false;
        }
    }
    if (layoutCode.includes("G")) {
        if (
            layoutCode.includes("T") ||
            layoutCode.includes("B") ||
            layoutCode.includes("C")
        ) {
            return false;
        }
    }
    return (
        layoutCode.includes("T") ||
        layoutCode.includes("B") ||
        layoutCode.includes("C") ||
        layoutCode.includes("G")
    );
}

function resetForm() {
    const fields = [
        "mounting", "width", "drop", "louvresize", "semiinsidemount",
        "midrailheight1", "midrailheight2", "midrailcritical", "panelqty", "joinedpanels", "hingecolour", "customheaderlength", "layoutcode", "layoutcodecustom", "frametype", "frameleft",
        "frameright", "frametop", "framebottom", "bottomtracktype", "buildout", "samesizepanel", "gap1", "gap2", "gap3", "gap4", "gap5", "horizontaltpostheight",
        "horizontaltpost", "tiltrodtype", "tiltrodsplit", "splitheight1", "splitheight2", "reversehinged",
        "pelmetflat", "extrafascia", "hingesloose", "notes", "markup"
    ];

    function resetFields() {
        fields.forEach(id => {
            let element = document.getElementById(id);
            if (element) {
                if (element.tagName === "SELECT") {
                    element.selectedIndex = 0;
                } else {
                    element.value = "";
                }
            }
        });
    }
    resetFields();
}

function showInfo(type) {
    let title;
    let spanInfo;
    if (type === "Tiltrod Type") {
        title = "Tiltrod Type Information";
        spanInfo = "<b>Easy Tilt</b>: Internal rack and pinion.<br />";
    } else if (type === "Gap") {
        title = "T-Post / Gap / Bay / Corner Location Information";
        info =
            "The factory will make all panels within an opening the same width unless otherwise indicated. If specific T-post locations are required, enter the measurement from the far left-hand side to the centre of the T-post measurement. The factory will make the panels to fit in between these posts.";
    }
    document.getElementById("modalTitle").innerHTML = title;
    document.getElementById("spanInfo").innerHTML = spanInfo;
}