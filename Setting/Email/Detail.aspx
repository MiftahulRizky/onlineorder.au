<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Email_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Detail Mail Configuration" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Mail Configuration</h2>
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
                            <h3 class="card-title">Detail Mail Configuration</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">APP NAME</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlAppId" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">MAIL NAME</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">SERVER</label>
                                <div class="col-lg-7 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtServer" CssClass="form-control" placeholder="Server ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">HOST</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtHost" CssClass="form-control" placeholder="Host ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">PORT</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPort" CssClass="form-control" placeholder="Port ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">MAIL ACCOUNT</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtAccount" CssClass="form-control" placeholder="Mail Account ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">MAIL PASSWSORD</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPassword" CssClass="form-control" placeholder="Mail Password ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">MAIL ALIAS</label>
                                <div class="col-lg-7 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtAlias" CssClass="form-control" placeholder="Mail Alias ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">MAIL SUBJECT</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtSubject" CssClass="form-control" placeholder="Mail Subject ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">MAIL TO</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtTo" CssClass="form-control" placeholder="Mail To ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                                    <small class="form-hint">* Split email with dot comma (;)</small>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">MAIL CC</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtCc" CssClass="form-control" placeholder="Mail CC ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                                    <small class="form-hint">* Split email with dot comma (;)</small>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">MAIL BCC</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtBcc" CssClass="form-control" placeholder="Mail BCC ..." autocomplete="off" TextMode="MultiLine" Height="100px" style="resize:none;"></asp:TextBox>
                                    <small class="form-hint">* Split email with dot comma (;)</small>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">ENABLE SSL</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlSSL" CssClass="form-select">
                                        <asp:ListItem Value="0" Text="FALSE"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="TRUE"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">DEFAULT CREDENTIALS</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlCredentials" CssClass="form-select">
                                        <asp:ListItem Value="0" Text="FALSE"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="TRUE"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">DESCRITPION</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>

                             <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">ACTIVE</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                        <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="NO"></asp:ListItem>
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
                            <h3 class="card-title">Information</h3>
                        </div>

                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })

        const element = document.querySelectorAll(".form-control, .form-select");
        element.forEach(e => {
            e.addEventListener("keyup", (e) => {
                e.target.classList.remove("is-invalid");
            })
        })
        
        // Function untuk menampilkan pesan error dari code-behind
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
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblAppId"></asp:Label>
        
        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE MailConfiguration SET AppId=@AppId, Name=@Name, Host=@Host, Port=@Port, EnableSsl=@EnableSsl, Server=@Server, Alias=@Alias, Account=@Account, Password=@Password, UseDefaultCredentials=@UseDefaultCredentials, Subject=@Subject, [To]=@To, Cc=@Cc, Bcc=@Bcc, Description=@Description, Active=@Active WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblAppId" Name="AppId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtHost" Name="Host" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtPort" Name="Port" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlSsl" Name="EnableSsl" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtServer" Name="Server" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtAlias" Name="Alias" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtAccount" Name="Account" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtPassword" Name="Password" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlCredentials" Name="UseDefaultCredentials" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtSubject" Name="Subject" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtTo" Name="To" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCc" Name="Cc" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtBcc" Name="Bcc" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>