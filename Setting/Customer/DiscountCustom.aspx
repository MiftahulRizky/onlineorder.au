<%@ Page Language="VB" AutoEventWireup="false" CodeFile="DiscountCustom.aspx.vb" Inherits="Setting_Customer_DiscountCustom" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Custom Customer Discount" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Order</div>
                    <h2 runat="server">Customer Discount</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-7 col-sm-12 col-md-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Custom Discount</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-4 col-form-label required">DISCOUNT TYPE</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlDiscountType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDiscountType_SelectedIndexChanged">
                                        <%--<asp:ListItem Value="Product" Text="PRODUCT"></asp:ListItem>--%>
                                        <asp:ListItem Value="Fabric" Text="FABRIC"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divDesignType">
                                <label class="col-lg-4 col-form-label required">PRODUCT TYPE</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlDesignType" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divFabricType">
                                <label class="col-lg-4 col-form-label required">FABRIC TYPE</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlFabricType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFabricType_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divFabricColour">
                                <label class="col-lg-4 col-form-label required">FABRIC COLOUR</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:ListBox runat="server" ID="lbFabricColour" CssClass="form-select" ClientIDMode="Static" SelectionMode="Multiple"></asp:ListBox>
                                    <small class="form-hint">* Leave this blank, if the discount applies to all fabric colors.</small>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divCustomProduct">
                                <label class="col-lg-4 col-form-label required">CUSTOM PRODUCT</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:ListBox runat="server" ID="lbFabricProduct" CssClass="form-select" ClientIDMode="Static" SelectionMode="Multiple"></asp:ListBox>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divPeriod">
                                <label class="col-lg-4 col-form-label required">PERIOD</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtStartDate" TextMode="Date" CssClass="form-control" placeholder="Start Date ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtEndDate" TextMode="Date" CssClass="form-control" placeholder="To Date ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divFinalDiscount">
                                <label class="col-lg-4 col-form-label required">FINAL DISCOUNT</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlFinalDiscount" CssClass="form-select">
                                        <asp:ListItem Value="0" Text=""></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 mt-5 row">
                                <label class="col-lg-4 col-form-label required">DISCOUNT</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <div class="input-group">
                                         <asp:TextBox runat="server" ID="txtDiscount" CssClass="form-control" placeholder="Discount ..." autocomplete="off"></asp:TextBox>
                                         <span class="percent input-group-text">%</span>
                                     </div>
                                     <small class="form-hint">* Please use decimal separator with (.)</small>
                                </div>
                            </div>.
                            
                            <div class="row" runat="server" id="divError">
                                <div class="col-12">
                                    <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                        <div class="d-flex">
                                            <div>
                                                <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                            </div>
                                            <div>
                                                <span runat="server" id="msgError"></span>
                                            </div>
                                        </div>
                                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-center">
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            loaderFadeOut();
            function initializeTomSelect(elementId) {
                var el = document.getElementById(elementId);
                if (window.TomSelect && el) {
                    new TomSelect(el, {
                        copyClassesToDropdown: false,
                        dropdownParent: "body",
                        controlInput: "<input>",
                        render: {
                            item: renderItem,
                            option: renderItem
                        }
                    });
                }
            }

            function renderItem(data, escape) {
                return data.customProperties
                    ? `<div><span class="dropdown-item-indicator">${data.customProperties}</span>${escape(data.text)}</div>`
                    : `<div>${escape(data.text)}</div>`;
            }

            initializeTomSelect("lbFabricColour");
            initializeTomSelect("lbFabricProduct");
        });
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>
</asp:Content>