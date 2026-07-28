<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Report.aspx.vb" Inherits="Report" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Report" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Report</h2>
                </div>
            </div>
        </div>
    </div>
    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-7">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Generate Report</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">REPORT TYPE</label>
                                <div class="col-lg-7 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlReportType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlReportType_SelectedIndexChanged">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="BlindsDaily" Text="BLINDS DAILY" hidden></asp:ListItem>
                                        <asp:ListItem Value="ShuttersDaily" Text="PANORAMA & EVOLVE DAILY"></asp:ListItem>
                                        <asp:ListItem Value="ShuttersOrder" Text="PANORAMA ORDERS"></asp:ListItem>
                                        <asp:ListItem Value="ShuttersMonth" Text="SHUTTERS MONTH TO MONTH" hidden></asp:ListItem>
                                        <asp:ListItem Value="" Text="" ></asp:ListItem>
                                        <asp:ListItem Value="EvolveOrder" Text="EVOLVE ORDERS"></asp:ListItem>
                                        <asp:ListItem Value="Customer" Text="CUSTOMERS" hidden></asp:ListItem>
                                        <asp:ListItem Value="Fabric" Text="FABRICS" hidden></asp:ListItem>
                                        <asp:ListItem Value="Newsletter" Text="NEWSLETTER" hidden></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label required">FILE TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlFileType" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divAdditional">
                                <asp:Label runat="server" ID="lblAdditional" CssClass="col-lg-3 col-form-label"></asp:Label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlAdditional" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="Yes" Text="YES"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divStartDate">
                                <label class="col-lg-3 col-form-label required">START DATE</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtStartDate" TextMode="Date" CssClass="form-control" placeholder="Start To ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row" runat="server" id="divEndDate">
                                <label class="col-lg-3 col-form-label required">END DATE</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtEndDate" TextMode="Date" CssClass="form-control" placeholder="Date To ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row mt-6" runat="server" id="divError">
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

                        <div class="card-footer text-start">
                            <asp:Button runat="server" ID="btnSubmit" Text="Generate Report" CssClass="btn btn-success" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblPeriod"></asp:Label>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
             loaderFadeOut();
        })
    </script>
</asp:Content>