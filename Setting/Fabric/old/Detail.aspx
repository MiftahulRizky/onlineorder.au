<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Detail.aspx.vb" Inherits="Setting_Fabric_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Detail Fabric" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Fabric</h2>
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
                            <h3 class="card-title">Detail Fabric</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">FABRIC ID</label>
                                <div class="col-md-3 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtId" CssClass="form-control" placeholder="Fabric ID ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-md-3 col-form-label">FABRIC NAME</label>
                                <div class="col-md-7 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Fabric Name ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">FABRIC TYPE</label>
                                <div class="col-md-4 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtType" CssClass="form-control" placeholder="Fabric Type ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-md-3 col-form-label">FABRIC COLOUR</label>
                                <div class="col-md-4 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtColour" CssClass="form-control" placeholder="Fabric Colour ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">FABRIC WIDTH</label>
                                <div class="col-md-5 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtWidth" CssClass="form-control" placeholder="Fabric Width ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-md-3 col-form-label">FABRIC GROUP</label>
                                <div class="col-md-5 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtGroup" CssClass="form-control" placeholder="Fabric Group ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                            
                            <div class="mb-6 row">
                                <label class="col-md-3 col-form-label">DESIGN TYPE</label>
                                <div class="col-md-5 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlDesignId" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">DESCRIPTION</label>
                                <div class="col-md-8 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-md-3 col-form-label">ACTIVE</label>
                                <div class="col-md-2 col-sm-12">
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
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblFabricIdOri"></asp:Label>

        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Fabrics SET DesignId=@DesignId, Name=@Name, Type=@Type, Colour=@Colour, Width=@Width, [Group]=@Group, Description=@Description, Active=@Active WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="txtId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlDesignId" Name="DesignId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtType" Name="Type" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtColour" Name="Colour" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtGroup" Name="Group" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>