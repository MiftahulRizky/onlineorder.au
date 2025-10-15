<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Store.aspx.vb" Inherits="Account_Store" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Account Store" %>

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
                                <a runat="server" href="~/account/" class="list-group-item list-group-item-action d-flex align-items-center">My Account</a>
                                <a runat="server" href="~/account/store" class="list-group-item list-group-item-action d-flex align-items-center active">My Store</a>
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
                            
                            <h3 class="card-title mt-4">Business Profile</h3>
                            <div class="row g-2">
                                <div class="col-3">
                                    <div class="form-label required">Store ID</div>
                                    <asp:TextBox runat="server" ID="txtId" CssClass="form-control" autocomplete="off" placeholder="Store Id ......."></asp:TextBox>
                                </div>
                                <div class="col-9">
                                    <div class="form-label required">Store Name</div>
                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" autocomplete="off" placeholder="Store Name ......."></asp:TextBox>
                                </div>
                                
                            </div>

                            <h3 class="card-title mt-4">Contact</h3>
                            <div class="row g-3">
                                <div class="col-md">
                                    <div class="form-label">Phone</div>
                                    <asp:TextBox runat="server" ID="txtPhone" CssClass="form-control" autocomplete="off" placeholder="Phone ......."></asp:TextBox>
                                </div>

                                <div class="col-md">
                                    <div class="form-label">Fax</div>
                                    <asp:TextBox runat="server" ID="txtFax" CssClass="form-control" autocomplete="off" placeholder="Fax ......."></asp:TextBox>
                                </div>

                                <div class="col-md">
                                    <div class="form-label">A B N</div>
                                    <asp:TextBox runat="server" ID="txtAbn" CssClass="form-control" autocomplete="off" placeholder="A B N ......."></asp:TextBox>
                                </div>
                            </div>

                            <h3 class="card-title mt-4">Email</h3>
                            <p class="card-subtitle">This contact will be shown to others publicly, so choose it carefully.</p>
                            <div>
                                <div class="row">
                                    <div class="col-5">
                                        <asp:TextBox runat="server" ID="txtEmail" TextMode="Email" CssClass="form-control" autocomplete="off" placeholder="Email ......."></asp:TextBox>
                                    </div>
                                    <div class="col-auto">
                                        <asp:Button runat="server" ID="btnEmail" CssClass="btn" Text="Change" OnClick="btnEmail_Click" />
                                    </div>
                                </div>
                            </div>

                            <h3 class="card-title mt-4">Address & Terms</h3>
                            <div class="row g-2">
                                <div class="col-md">
                                    <div class="form-label">Address</div>
                                    <asp:TextBox runat="server" ID="txtAddress" TextMode="MultiLine" CssClass="form-control" Height="80px" autocomplete="off" placeholder="Address ......." style="resize:none;"></asp:TextBox>
                                </div>

                                <div class="col-md">
                                    <div class="form-label">Terms</div>
                                    <asp:TextBox runat="server" ID="txtTerms" TextMode="MultiLine" CssClass="form-control" Height="80px" autocomplete="off" placeholder="Terms ......." style="resize:none;"></asp:TextBox>
                                </div>
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
        <asp:Label runat="server" ID="lblMailOld"></asp:Label>

        <asp:SqlDataSource ID="sdsEmail" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Email SET Email=@Email WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="txtId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtEmail" Name="Email" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Stores SET Name=@Name, Address=@Address, Phone=@Phone, Email=@Email, Fax=@Fax, ABN=@Abn, Terms=@Terms WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="txtId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtAddress" Name="Address" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtPhone" Name="Phone" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtEmail" Name="Email" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtFax" Name="Fax" PropertyName="Text" />

                <asp:ControlParameter ControlID="txtAbn" Name="Abn" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTerms" Name="Terms" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>