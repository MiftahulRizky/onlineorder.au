<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Product_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Product" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .datagridTitle{ font-size:14px; }
        .datagridContent { font-size:16px; }
    </style>
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">
                        <a runat="server" href="~/setting/product" class="text-decoration-none">Product</a>
                    </h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-8 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Add Product</h3>
                        </div>

                        <div class="card-body">
                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">DESIGN TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlDesignId" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesignId_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">BLIND TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlBlindId" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">PRODUCT NAME</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">TUBE TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlTubeType" CssClass="form-select mb-2"></asp:DropDownList>
                                        <a href="#" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalTube">Add New</a>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">CONTROL TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlControlType" CssClass="form-select mb-2"></asp:DropDownList>
                                        <a href="#" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalControl">Add New</a>
                                    </div>
                                </div>
                                
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">COLOUR TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlColourType" CssClass="form-select mb-2"></asp:DropDownList>
                                        <a href="#" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalColour">Add New</a>
                                    </div>
                                </div>
                            </div>

                           <div class="datagrid mb-5">
                               <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">DESCRITPION</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                    </div>
                                </div>
                           </div>

                            <div class="datagrid mb-5">
                                <div class="datagrid-item">
                                     <div class="datagrid-title datagridTitle">ACTIVE</div>
                                     <div class="datagrid-content datagridContent">
                                         <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select" Width="25%">
                                            <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                            <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                                        </asp:DropDownList>
                                     </div>
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

                        <div class="card-footer text-start">
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-4 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Information</h3>
                        </div>

                        <div class="card-body">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalTube" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Tube Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="form-label">Name :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" ID="txtTubeName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Description :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtTubeDescription" Height="100px" CssClass="form-control" ReadOnly="true" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Active :</label>
                        <div class="col-lg-4 col-md-12 col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlTubeActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn me-auto" data-bs-dismiss="modal">Close</button>
                    <asp:Button runat="server" ID="btnSubmitTube" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitTube_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalControl" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Control Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="form-label">Name :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" ID="txtControlName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Description :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtControlDescription" Height="100px" CssClass="form-control" ReadOnly="true" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Active :</label>
                        <div class="col-lg-4 col-md-12 col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlControlActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn me-auto" data-bs-dismiss="modal">Close</button>
                    <asp:Button runat="server" ID="btnSubmitControl" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitControl_Click" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal modal-blur fade" id="modalColour" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Colour Type</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <label class="form-label">Name :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" ID="txtColourName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Description :</label>
                        <div class="col-lg-12 col-md-12 col-sm-12">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtColourDescription" Height="100px" CssClass="form-control" ReadOnly="true" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-3 row">
                        <label class="form-label">Active :</label>
                        <div class="col-lg-4 col-md-12 col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlColourActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn me-auto" data-bs-dismiss="modal">Close</button>
                    <asp:Button runat="server" ID="btnSubmitColour" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitColour_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>