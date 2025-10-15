<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Default.aspx.vb" Inherits="Account_Default" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Account" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Account Settings</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="card">
                <div class="row g-0">
                    <div class="col-12 col-md-3 border-end">
                        <div class="card-body">
                            <h4 class="subheader">Business settings</h4>
                            <div class="list-group list-group-transparent">
                                <a runat="server" href="~/account/" class="list-group-item list-group-item-action d-flex align-items-center active">My Account</a>
                                <a runat="server" href="~/account/store" class="list-group-item list-group-item-action d-flex align-items-center">My Store</a>
                            </div>
                            <h4 class="subheader mt-4">Experience</h4>
                            <div class="list-group list-group-transparent">
                                <a runat="server" href="~/account/feedback" class="list-group-item list-group-item-action">Give Feedback</a>
                            </div>
                        </div>
                    </div>

                    <div class="col-12 col-md-9 d-flex flex-column">
                        <div class="card-body">
                            <h2 class="mb-4">My Account</h2>
                            <h3 class="card-title">Profile Details</h3>
                            <div class="row align-items-center">
                                <div class="col-auto"><span class="avatar avatar-xl rounded-circle" style="background-image: url(http://10.0.209.168:8888/Content/static/new-avatar.png)"></span>
                                </div>
                                <%--<div class="col-auto">
                                    <a href="#" class="btn">Change avatar</a>
                                </div>
                                <div class="col-auto">
                                    <a href="#" class="btn btn-ghost-danger">Delete avatar</a>
                                </div>--%>
                            </div>
                            
                            <h3 class="card-title mt-4">Business Profile</h3>
                            <div class="row g-2">
                                <div class="col-md">
                                    <div class="form-label required">Full Name</div>
                                    <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" autocomplete="off" placeholder="Full Name ...."></asp:TextBox>
                                </div>
                            </div>

                            <h3 class="card-title mt-4">System Profile</h3>
                            <div class="row g-3">
                                <div class="col-md">
                                    <div class="form-label required">User Name</div>
                                    <asp:TextBox runat="server" ID="txtUserName" CssClass="form-control" autocomplete="off" placeholder="UserName ...."></asp:TextBox>
                                </div>

                                <div class="col-md">
                                    <div class="form-label required">Role Access</div>
                                    <asp:DropDownList runat="server" ID="ddlRole" CssClass="form-select"></asp:DropDownList>
                                </div>

                                <div class="col-md">
                                    <div class="form-label required">Level Access</div>
                                    <asp:DropDownList runat="server" ID="ddlLevel" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <h3 class="card-title mt-4">Email</h3>
                            <p class="card-subtitle">
                                This contact will be used by the system for automatic email sending of the order you submitted and some information that we want to convey to users.
                            </p>
                            <div>
                                <div class="row">
                                    <div class="col-5">
                                        <asp:TextBox runat="server" ID="txtEmail" TextMode="Email" CssClass="form-control" autocomplete="off" placeholder="Email ...."></asp:TextBox>
                                    </div>
                                    <div class="col-auto">
                                        <asp:Button runat="server" ID="btnEmail" CssClass="btn" Text="Change" OnClick="btnEmail_Click" />
                                    </div>
                                </div>
                            </div>

                            <h3 class="card-title mt-4">Password</h3>
                            <p class="card-subtitle">You can set a permanent password if you don't want to use temporary login codes.</p>
                            <div>
                                <a runat="server" href="~/account/password" class="btn">
                                    Set new password
                                </a>
                            </div>

                        </div>

                        <div class="card-body border-bottom py-3" runat="server" id="divError">
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

                        <div class="card-footer bg-transparent mt-auto">
                            <div class="btn-list justify-content-end">
                                <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                                <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblUserId"></asp:Label>
        <asp:Label runat="server" ID="lblEmailOld"></asp:Label>

        <asp:SqlDataSource ID="sdsEmail" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Users SET Email=@Email WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtEmail" Name="Email" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsMember" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET UserName=@UserName WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtUserName" Name="UserName" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsUsers" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Users SET FullName=@FullName, Email=@Email WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtFullName" Name="FullName" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtEmail" Name="Email" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>