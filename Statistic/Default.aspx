<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Statistics_Default" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Statistics" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Statistics Order</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-4 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Input Form</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-4 row">
                                <div class="col-6">
                                    <label class="form-label required">COMPANY</label>
                                    <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="mb-5 row">
                                <div class="col-6">
                                    <label class="form-label required">FROM DATE</label>
                                    <asp:TextBox runat="server" ID="txtFromDate" TextMode="Date" CssClass="form-control" placeholder="From Date ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-6">
                                    <label class="form-label required">TO DATE</label>
                                    <asp:TextBox runat="server" ID="txtToDate" TextMode="Date" CssClass="form-control" placeholder="To Date ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div runat="server" id="divError" class="alert alert-important alert-danger alert-dismissible" role="alert">
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

                        <div class="card-footer text-end">
                            <asp:LinkButton runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnSubmit_Click" >
                                <i class="bi bi-search me-2"></i> Submit
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger " OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-rotate-left me-2"></i>Cancel
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-lg-8 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <ul class="nav nav-tabs card-header-tabs" data-bs-toggle="tabs">
                                <li class="nav-item">
                                    <a href="#tabDesign" class="nav-link active" data-bs-toggle="tab">
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon me-2" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M5 12l-2 0l9 -9l9 9l-2 0" /><path d="M5 12v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2 -2v-7" /><path d="M9 21v-6a2 2 0 0 1 2 -2h2a2 2 0 0 1 2 2v6" /></svg>
                                        Job Order
                                    </a>
                                </li>

                                <li class="nav-item">
                                    <a href="#tabControl" class="nav-link" data-bs-toggle="tab">
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon me-2" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M5 12l-2 0l9 -9l9 9l-2 0" /><path d="M5 12v7a2 2 0 0 0 2 2h10a2 2 0 0 0 2 -2v-7" /><path d="M9 21v-6a2 2 0 0 1 2 -2h2a2 2 0 0 1 2 2v6" /></svg>
                                        Item Order
                                    </a>
                                </li>
                            </ul>
                        </div>

                        <div class="card-body">
                            <div class="tab-content">
                                <div class="tab-pane active show" id="tabDesign">
                                    <h4>Job Order Tab</h4>
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvHeader" CssClass="table table-vcenter card-table" AutoGenerateColumns="false">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="StoreName" HeaderText="STORE NAME" />
                                                <asp:BoundField DataField="OrderNo" HeaderText="ORDER NO" />
                                                <asp:BoundField DataField="OrderCust" HeaderText="ORDER CUST" />
                                                <asp:BoundField DataField="StoreCompany" HeaderText="COMPANY" />
                                                <asp:BoundField DataField="SumItem" HeaderText="TOTAL ITEM" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>

                                <div class="tab-pane" id="tabControl">
                                    <h4>Item Order tab</h4>
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvDetail" CssClass="table table-vcenter card-table" AutoGenerateColumns="false">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="BlindType" HeaderText="PRODUCT" />
                                                <asp:BoundField DataField="CountItem" HeaderText="TOTAL" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            loaderFadeOut();
        })
        
        document.querySelectorAll(".form-control, .form-select").forEach((el) => {
            el.addEventListener("change", function (e) {
                e.target.classList.remove("is-invalid");
            })
            el.addEventListener("input", function (e) {
                e.target.classList.remove("is-invalid");
            })
        })

        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                html: msg,
                customClass: {
                    popup: isDark ? "bg-dark text-white" : "bg-white text-dark"
                }
            });
        }
    </script>
</asp:Content>
