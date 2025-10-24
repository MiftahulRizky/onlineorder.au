<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Add.aspx.vb" Inherits="Setting_Chain_Add" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Add Chain" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Chain</h2>
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
                            <h3 class="card-title">Add Chain</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">ID</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtId" TextMode="Number" CssClass="form-control" placeholder="Id ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label required">CHAIN NAME</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">CHAIN COLOUR</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtColour" CssClass="form-control" placeholder="Colour ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label required">CHAIN LENGTH</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlLength" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="Custom" Text="CUSTOM"></asp:ListItem>
                                        <asp:ListItem Value="100" Text="100 mm"></asp:ListItem>
                                        <asp:ListItem Value="500" Text="500 mm"></asp:ListItem>
                                        <asp:ListItem Value="600" Text="600 mm"></asp:ListItem>
                                        <asp:ListItem Value="800" Text="800 mm"></asp:ListItem>
                                        <asp:ListItem Value="1000" Text="1000 mm"></asp:ListItem>
                                        <asp:ListItem Value="1200" Text="1200 mm"></asp:ListItem>
                                        <asp:ListItem Value="1500" Text="1500 mm"></asp:ListItem>
                                        <asp:ListItem Value="1800" Text="1800 mm"></asp:ListItem>
                                        <asp:ListItem Value="2000" Text="2000 mm"></asp:ListItem>
                                        <asp:ListItem Value="2200" Text="2200 mm"></asp:ListItem>
                                        <asp:ListItem Value="2500" Text="2500 mm"></asp:ListItem>
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
        <asp:Label runat="server" ID="lblName"></asp:Label>
        <asp:Label runat="server" ID="lblBlindId"></asp:Label>

        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO Chains VALUES (@Id, @Name, @Colour, @Length, '', '', '', @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="txtId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtColour" Name="Colour" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlLength" Name="Length" PropertyName="SelectedItem.Value" />
                <%-- <asp:ControlParameter ControlID="txtBlind" Name="BlindId" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtMechanism" Name="Mechanism" PropertyName="Text" /> --%>
                <asp:ControlParameter ControlID="txtDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
    </script>
</asp:Content>