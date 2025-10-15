<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Detail.aspx.vb" Inherits="Setting_Member_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Detail Membership" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Memberships</h2>
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
                            <h3 class="card-title" runat="server" id="cardTitle">Detail Membership</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row" runat="server" id="divAppId">
                                <label class="col-lg-3 col-form-label required">APPLICATION ID</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlAppId" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">ROLE MEMBER</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlRole" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-lg-3 col-form-label required">LEVEL MEMBER</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlLevel" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">USERNAME</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtUserName" CssClass="form-control" placeholder="UserName ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            
                            <div class="mb-1 mt-6 row">
                                <p style="color:red;">
                                    <b><u>USER DATA</u></b>
                                </p>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">STORE</label>
                                <div class="col-lg-7 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlStore" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">FIND BY STORE ID</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtStore" CssClass="form-control " placeholder="Find store id ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:LinkButton runat="server" ID="btnFindStore" CssClass="btn btn-primary " Text="Find" OnClick="btnFindStore_Click" >
                                        <i class="bi bi-search me-2"></i> Find
                                    </asp:LinkButton>
                                </div>
                            </div>

                            <div class="mb-3 mt-4 row">
                                <label class="col-lg-3 col-form-label">FULL NAME</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" placeholder="Full Name ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">E-MAIL</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="Email" ID="txtEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">PHONE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPhone" CssClass="form-control" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-1 mt-6 row">
                                <p style="color:red;"><b><u>ACCESS</u></b></p>
                            </div>

                             <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">MARK UP</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlMarkUp" CssClass="form-select">
                                        <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">PRICING / COST</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlPrice" CssClass="form-select">
                                        <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                    </asp:DropDownList>
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
                            <asp:LinkButton runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary " OnClick="btnSubmit_Click" >
                                <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger " OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-rotate-left me-2"></i> Cancel
                            </asp:LinkButton>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Change Password</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-4 col-form-label">NEW PASSWORD</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" placeholder="Password ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-4 col-form-label">CONF PASSWORD</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtCPassword" TextMode="Password" CssClass="form-control" placeholder="Confirm Password ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row" runat="server" id="divErrorPassword">
                                <div class="col-lg-12">
                                    <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                        <div class="d-flex">
                                            <div>
                                                <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                            </div>
                                            <div>
                                                <span runat="server" id="msgErrorPassword"></span>
                                            </div>
                                        </div>
                                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer text-end">
                            <asp:LinkButton runat="server" ID="btnSubmitPassword" Text="Submit" CssClass="btn btn-primary " OnClick="btnSubmitPassword_Click" >
                                <i class="fa-solid fa-cloud-arrow-up me-2"></i> Submit
                            </asp:LinkButton>
                            <asp:LinkButton runat="server" ID="btnCancelPassword" Text="Cancel" CssClass="btn btn-danger " OnClick="btnCancel_Click" >
                                <i class="fa-solid fa-rotate-left me-2"></i> Cancel
                            </asp:LinkButton>
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

        document.querySelectorAll(".form-control, .form-select").forEach(e => {
            e.addEventListener("keyup", (e) => {
                e.target.classList.remove("is-invalid");
            })
            e.addEventListener("change", (e) => {
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblUserId"></asp:Label>
        <asp:Label runat="server" ID="lblPasswordHash"></asp:Label>

        <asp:SqlDataSource ID="sdsPassword" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET Password=@Password WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPasswordHash" Name="Password" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsPageMember" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Memberships SET ApplicationId=@AppId, UserName=@UserName, RoleId=@RoleId, LevelId=@LevelId WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlAppId" Name="AppId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlRole" Name="RoleId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlLevel" Name="LevelId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtUserName" Name="UserName" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsPageUsers" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Users SET StoreId=@StoreId, FullName=@FullName, Phone=@Phone, Email=@Email, MarkUp=@MarkUp, Price=@Price WHERE UserId=@UserId">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblUserId" Name="UserId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlStore" Name="StoreId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtFullName" Name="FullName" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtPhone" Name="Phone" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtEmail" Name="Email" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlMarkUp" Name="MarkUp" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlPrice" Name="Price" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>