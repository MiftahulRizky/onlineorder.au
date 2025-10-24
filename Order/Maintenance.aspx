<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Maintenance.aspx.vb" Inherits="Order_Maintenance" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Maintenance Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title" runat="server" id="pageTitle">Maintenance Product</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" runat="server" id="cardTitle"></h3>
                        </div>
                        <div class="card-body">
                            <div runat="server" id="divError" class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span style="font-size:large;">
                                            <b>
                                                At this time, roller products cannot be used yet. <br />
                                                Please contact our office for further details. <br /><br />

                                                We apologize for this inconvenience and thank you.
                                            </b>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-start">
                            <asp:Button runat="server" ID="btnSubmit" Text="Back to your order detail page" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>