<%@ Page Language="VB" AutoEventWireup="true" CodeFile="View.aspx.vb" Inherits="Tutorial_View" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="View Tutorial" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Read More Tutorial</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" runat="server" id="cardTitle"></h3>
                            <div class="card-actions">
                                <asp:LinkButton runat="server" ID="btnFinish" CssClass="btn btn-primary " Text="Finish" OnClick="btnFinish_Click" >
                                    <i class="bi bi-check-lg me-2"></i>
                                    Finish
                                </asp:LinkButton>
                            </div>
                        </div>

                        <div class="card-body py-3" runat="server" id="divError">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round">
                                            <path stroke="none" d="M0 0h24v24H0z" fill="none"></path>
                                            <path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0"></path>
                                            <path d="M12 8v4"></path>
                                            <path d="M12 16h.01"></path>
                                        </svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgError"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <embed runat="server" id="embPdf" type="application/pdf" width="100%" height="1860" />
                        </div>

                        <div class="row">
                            <div class="col-lg-12">
                                <iframe runat="server" id="iframePdf"></iframe>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', () => {
            loaderFadeOut();
        })
    </script>
</asp:Content>