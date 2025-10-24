<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Password.aspx.vb" Inherits="Account_Password" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Change Password" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <h2 class="page-title">Change Password</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-6 col-sm-12 col-md-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" runat="server">Password Form</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-4 col-form-label">NEW PASSWORD</label>
                                <div class="col">
                                    <asp:TextBox runat="server" ID="txtNewPass" TextMode="Password" CssClass="form-control" placeholder="Password ..."></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-4 col-form-label">CONFIRM NEW PASSWORD</label>
                                <div class="col">
                                    <asp:TextBox runat="server" ID="txtCNewPass" TextMode="Password" CssClass="form-control" placeholder="Confirm Password ..."></asp:TextBox>
                                </div>
                            </div>

                            <div class="row" runat="server" id="divError">
                                <div class="col-lg-12">
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

                        <div class="card-footer text-end">
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-4 col-sm-12 col-md-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" runat="server">Information</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <div class="col-12 markdown">
                                    <ol>
                                        <li>If the password is successfully changed, your account will be logged out automatically and you will be prompted to log in again</li>
                                    </ol>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblUserId"></asp:Label>
        <asp:Label runat="server" ID="lblPassword"></asp:Label>
        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET Password=@Password WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPassword" Name="Password" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
