<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Export_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Export Order" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Export</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-6 col-md-12 col-sm-12">
                    <div class="card mb-5">
                        <div class="card-header">
                            <h3 class="card-title">Export Data Order</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">EXPORT TYPE</label>
                                <div class="col-md-3 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlExport" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="XML" Text="XML"></asp:ListItem>
                                        <asp:ListItem Value="PDF" Text="PDF"></asp:ListItem>
                                        <asp:ListItem Value="Excel" Text="MS EXCEL"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                            </div>
                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">ORDER DATA</label>
                                <div class="col-md-3 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlData" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="headersp" Text="HEADER SP"></asp:ListItem>
                                        <asp:ListItem Value="detailsp" Text="DETAIL SP"></asp:ListItem>
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="headersg" Text="HEADER SG"></asp:ListItem>
                                        <asp:ListItem Value="detailsg" Text="DETAIL SG"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">ORDER STATUS</label>
                                <div class="col-md-4 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                        <asp:ListItem Value="all" Text="ALL"></asp:ListItem>
                                        <asp:ListItem Value="draft" Text="DRAFT"></asp:ListItem>
                                        <asp:ListItem Value="new" Text="NEW ORDER"></asp:ListItem>
                                        <asp:ListItem Value="production" Text="IN PRODUCTION"></asp:ListItem>
                                        <asp:ListItem Value="hold" Text="HOLD"></asp:ListItem>
                                        <asp:ListItem Value="completed" Text="COMPLETED"></asp:ListItem>
                                        <asp:ListItem Value="canceled" Text="CANCELED"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">FROM - TO DATE</label>
                                <div class="col-md-4 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtFromDate" TextMode="Date" CssClass="form-control" placeholder="From Date ..." autocomplete="off"></asp:TextBox>
                                    <small class="form-hint">* From Date</small>
                                </div>
                                <div class="col-md-4 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtToDate" TextMode="Date" CssClass="form-control" placeholder="To Date ..." autocomplete="off"></asp:TextBox>
                                    <small class="form-hint">* To Date</small>
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
                                <i class="bi bi-search me-2"></i>Submit
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger " OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-rotate-left me-2"></i>Cancel
                            </asp:LinkButton>
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
