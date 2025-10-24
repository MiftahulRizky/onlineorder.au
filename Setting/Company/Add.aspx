<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Add.aspx.vb" Inherits="Setting_Company_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Company" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Company</h2>
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
                            <h3 class="card-title">Add Company</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">ID</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtId" CssClass="form-control" placeholder="Company Code ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">NAME</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Company Name ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">DESCRITPION</label>
                                <div class="col-lg-7 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>
                            
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">ACTIVE</label>
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

        const id = document.getElementById("MainContent_txtId");
        id.addEventListener("keyup", (e) => {
            e.target.classList.remove("is-invalid");
        })

        const name = document.getElementById("MainContent_txtName");
        name.addEventListener("keyup", (e) => {
            e.target.classList.remove("is-invalid");
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
        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO Company VALUES (@Id, @Name, @Description, @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="txtId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>