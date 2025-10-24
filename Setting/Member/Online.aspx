<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Online.aspx.vb" Inherits="Setting_Member_Online" MasterPageFile="~/Site.master"  MaintainScrollPositionOnPostback="true" Debug="true" Title="Online Membership" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Member Online</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">List Data</h3>
                            <div class="card-actions">
                                <asp:LinkButton runat="server" ID="btnFinish" CssClass="btn btn-primary " Text="Finish / Back" OnClick="btnFinish_Click" >
                                    <i class="bi bi-check-lg me-2"></i> Finish / Back
                                </asp:LinkButton>
                            </div>
                        </div>

                        <div class="card-body py-3">
                            <div class="d-flex">
                                <div class="ms-auto text-secondary">
                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                        <div class="ms-2 d-inline-block">
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control " placeholder="Search Data" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="ms-2 d-inline-block">
                                            <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn btn-primary " Text="Search" OnClick="btnSearch_Click" >
                                                <i class="fa-solid fa-magnifying-glass" style="margin-right: 0.3rem;"></i> Search
                                            </asp:LinkButton>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                        <div class="card-body py-3" runat="server" id="divError">
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
                        <div class="card-body py-3">
                            <div class="row row-cards">
                                <asp:ListView runat="server" ID="lvMember">
                                    <ItemTemplate>
                                        <div class="col-md-6 col-lg-3">
                                            <div class="card">
                                                <div class="card-body p-4 text-center">
                                                    <span class="avatar avatar-xl mb-3 rounded" style="background-image: url(../../Content/static/avatars.jpg)"></span>
                                                    <h3 class="m-0 mb-1"><a href="#"><%# Eval("UserName").ToString() %></a></h3>
                                                    <div class="text-secondary"><%# Eval("Access").ToString() %></div>
                                                    <div class="mt-3">
                                                        <span class="badge bg-purple-lt"><%# Eval("StoreName").ToString() %></span>
                                                    </div>
                                                </div>
                                                <div class="d-flex">
                                                    <a href='<%# String.Format("mailto:{0}", Eval("UserEmail").ToString()) %>' runat="server" target="_blank" class="card-btn">
                                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon me-2 text-muted" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 7a2 2 0 0 1 2 -2h14a2 2 0 0 1 2 2v10a2 2 0 0 1 -2 2h-14a2 2 0 0 1 -2 -2v-10z" /><path d="M3 7l9 6l9 -6" /></svg>
                                                        Email
                                                    </a>
                                                    <a href="#" class="card-btn">
                                                         <svg xmlns="http://www.w3.org/2000/svg" class="icon me-2 text-muted" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M5 4h4l2 5l-2.5 1.5a11 11 0 0 0 5 5l1.5 -2.5l5 2v4a2 2 0 0 1 -2 2a16 16 0 0 1 -15 -15a2 2 0 0 1 2 -2" /></svg>
                                                        Call
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:ListView>
                            </div>
                        </div>
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
