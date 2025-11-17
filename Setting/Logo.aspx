<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Logo.aspx.vb" Inherits="Setting_Logo" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.Master" Debug="true" Title="Logo" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Logo</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-6">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Your Logo</h3>
                        </div>
                        <div class="card-body">
                            <h2 class="mb-4">My Logo</h2>
                            <div class="row align-items-center">
                                <div class="col-auto">
                                    <asp:Image runat="server" ID="imgLogo" CssClass="d-block" AlternateText="My Logo" />
                                </div>
                            </div>
                            
                            <h3 class="card-title mt-6" runat="server" id="hLogo">Change Logo</h3>

                            <div class="row mb-6">
                                <div class="col-9">
                                    <asp:FileUpload runat="server" ID="fuLogo" CssClass="form-control" />
                                </div>
                                <div class="col-3">
                                    <asp:Button runat="server" ID="btnUpload" Text="Upload Logo" CssClass="btn btn-primary" OnClick="btnUpload_Click" />
                                </div>
                            </div>

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
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblLogo"></asp:Label>
        <asp:Label runat="server" ID="lblLogoOld"></asp:Label>

        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE CustomerQuotes SET Logo=@Logo WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblCustomerId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblLogo" Name="Logo" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
    </script>
</asp:Content>
