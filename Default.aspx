<%@ Page Title="Home Page" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeFile="Default.aspx.vb" Inherits="_Default" %>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="page-header">
            <div class="container-xl">
                <div class="row g-2 align-items-center">
                    <div class="col">
                        <h2 class="page-title" runat="server" id="pageTitle"></h2>
                    </div>
                </div>
            </div>
        </div>

        <div class="page-body">
            <div class="container-xl">
                <div class="row">
                    <div class="col-12">
                        <div class="card" runat="server" id="divNewsletter">
                            <div class="card-header">
                                <h3 class="card-title">Latest Update & News App</h3>
                            </div>

                            <div class="card-body">
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                            </div>

                            <div class="card-footer text-end"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <script type="text/javascript">
            document.addEventListener("DOMContentLoaded", () => {
                loaderFadeOut();
            })
        </script>
    </asp:Content>