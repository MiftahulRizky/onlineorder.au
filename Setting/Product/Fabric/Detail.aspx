<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Product_Fabric_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Fabric Detail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .datagridTitle{ font-size:14px; }
        .datagridContent { font-size:16px; }
    </style>

    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col-6">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">
                        <a runat="server" href="~/setting/product/fabric">Fabric</a>
                    </h2>
                </div>
                <div class="col-6 text-end">
                    <a href="#" runat="server" id="aEdit" class="btn btn-info" data-bs-toggle="modal" data-bs-target="#modalEdit">Edit</a>
                    <a href="#" runat="server" id="aDelete" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete</a>
                    <asp:Button runat="server" ID="btnLog" CssClass="btn btn-secondary" Text="Log" OnClick="btnLog_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row" runat="server" id="divError">
                <div class="col-12">
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

            <div class="row mb-3">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Detail Fabric</h3>
                        </div>

                        <div class="card-body">
                            <div class="datagrid">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">ID</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblId"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Fabric Name</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblName"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Type</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblType"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Price Group</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblGroup"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">COMPOSITION</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblComposition"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">FLAME RETERDANT</div>
                                    <div class="datagrid-content">
                                        <div runat="server" id="divFlameReterdantSuccess">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M9 12l2 2l4 -4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            Yes
                                        </div>
                                        <div runat="server" id="divFlameReterdantDanger">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-x text-danger">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M10 10l4 4m0 -4l-4 4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            No
                                        </div>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">PVC / LEAD FREE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblPvcLead"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">GREENDGUARD GOLD</div>
                                    <div class="datagrid-content datagridContent">
                                        <div runat="server" id="divGreenguardGoldSuccess">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M9 12l2 2l4 -4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            Yes
                                        </div>
                                        <div runat="server" id="divGreenguardGoldDanger">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-x text-danger">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M10 10l4 4m0 -4l-4 4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            No
                                        </div>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">WEIGHT</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblWeight"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">THICKNESS</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblThickness"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">NO RAIL ROAD</div>
                                    <div class="datagrid-content datagridContent">
                                        <div runat="server" id="divNoRailRoadSuccess">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M9 12l2 2l4 -4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            Yes
                                        </div>

                                        <div runat="server" id="divNoRailRoadDanger">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-x text-danger">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M10 10l4 4m0 -4l-4 4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            No
                                        </div>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">ACTIVE</div>
                                    <div class="datagrid-content datagridContent">
                                        <div runat="server" id="divActiveSuccess">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M9 12l2 2l4 -4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            Yes
                                        </div>

                                        <div runat="server" id="divActiveDanger">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-x text-danger">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M10 10l4 4m0 -4l-4 4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            No
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <br /><br />

                            <div class="datagrid">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">PRDOUCT</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblProduct"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">TUBE TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblTube"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">FACTORY</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblFactory"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <br />
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Fabric Colour</h3>
                            <div class="card-actions">
                                <asp:Button runat="server" ID="btnAddColour" CssClass="btn btn-orange" Text="Add Fabric Colour" OnClick="btnAddColour_Click" />
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvListColour" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="FABRIC COLOUR DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvListColour_PageIndexChanging" OnRowCommand="gvListColour_RowCommand">
                                    <RowStyle />
                                    <Columns>
                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                        <asp:BoundField DataField="BoeId" HeaderText="BOE ID" />
                                        <asp:BoundField DataField="Name" HeaderText="Name" />
                                        <asp:BoundField DataField="Colour" HeaderText="Colour" />
                                        <asp:BoundField DataField="Width" HeaderText="Width" />
                                        <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                        <asp:BoundField DataField="Active" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                            <ItemTemplate>
                                                <button class="btn btn-sm btn-pill btn-orange" data-bs-toggle="dropdown">Actions</button>
                                                <div class="dropdown-menu dropdown-menu-end">
                                                    <asp:LinkButton runat="server" ID="linkEditColour" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>' Visible='<%# VisibleAction() %>'></asp:LinkButton>
                                                    <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteColour" onclick='<%# String.Format("return showDeleteColour(`{0}`);", Eval("Id").ToString()) %>' visible='<%# VisibleAction() %>'>Delete</a>
                                                    <div class="dropdown-divider"></div>
                                                    <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkLogColour" Text="Logs" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                    <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                    <AlternatingRowStyle BackColor="White" />
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal modal-blur fade" id="modalEdit" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-full-width modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Fabric</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-4 row">
                        <div class="col-2">
                            <label class="form-label">ID</label>
                            <asp:TextBox runat="server" ID="txtId" CssClass="form-control" placeholder="Id ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-4">
                            <label class="form-label required">NAME</label>
                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                        
                        <div class="col-3">
                            <label class="form-label required">TYPE</label>
                            <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Blockout" Text="BLOCKOUT"></asp:ListItem>
                                <asp:ListItem Value="Light Filter" Text="LIGHT FILTER"></asp:ListItem>
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-3">
                            <label class="form-label required">GROUP</label>
                            <asp:TextBox runat="server" ID="txtGroup" CssClass="form-control" placeholder="Group ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-4 row">
                        <div class="col-3">
                            <label class="form-label required">COMPOSITION</label>
                            <asp:TextBox runat="server" ID="txtComposition" CssClass="form-control" placeholder="Composition ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-3">
                            <label class="form-label required">FLAME RETERDANT</label>
                            <asp:DropDownList runat="server" ID="ddlFlameReterdant" CssClass="form-select">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-3">
                            <label class="form-label required">PVC / LEAD FREE</label>
                            <asp:DropDownList runat="server" ID="ddlPvcLead" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="P/L" Text="P/L"></asp:ListItem>
                                <asp:ListItem Value="P" Text="P"></asp:ListItem>
                                <asp:ListItem Value="L" Text="L"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-3">
                            <label class="form-label required">GREENDGUARD GOLD</label>
                            <asp:DropDownList runat="server" ID="ddlGreenguardGold" CssClass="form-select">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-4 row">
                        <div class="col-3">
                            <label class="form-label required">WEIGHT</label>
                            <asp:TextBox runat="server" ID="txtWeight" TextMode="Number" CssClass="form-control" placeholder="Weight ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-3">
                            <label class="form-label required">THICKNESS</label>
                            <asp:TextBox runat="server" ID="txtThickness" CssClass="form-control" placeholder="Thickness ..." autocomplete="off"></asp:TextBox>
                        </div>

                        <div class="col-3">
                            <label class="form-label required">RAIL ROAD</label>
                            <asp:DropDownList runat="server" ID="ddlNoRailRoad" CssClass="form-select">
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-3">
                            <label class="form-label required">ACTIVE</label>
                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-6 row">
                        <div class="col-5">
                            <label class="form-label required">PRODUCT</label>
                            <asp:ListBox runat="server" ID="lbDesign" CssClass="form-select" SelectionMode="multiple" ClientIDMode="Static"></asp:ListBox>
                        </div>

                        <div class="col-5">
                            <label class="form-label">TUBE TYPE</label>
                            <asp:ListBox runat="server" ID="lbTube" CssClass="form-select" SelectionMode="multiple" ClientIDMode="Static"></asp:ListBox>
                        </div>

                        <div class="col-2">
                            <label class="form-label required">FACTORY</label>
                            <asp:DropDownList runat="server" ID="ddlFactory" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="JKT" Text="JKT"></asp:ListItem>
                                <asp:ListItem Value="Orion" Text="ORION"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorEdit">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorEdit"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancelEdit" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitEdit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitEdit_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalAddColour" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleColour"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-4 row">
                        <div class="col-4">
                            <label class="form-label">BOE ID</label>
                            <asp:TextBox runat="server" ID="txtBoeId" CssClass="form-control" placeholder="Id ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-8">
                            <label class="form-label required">Fabric Name</label>
                            <asp:TextBox runat="server" ID="txtFabricColourName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="mb-4 row">
                        <div class="col-5">
                            <label class="form-label required">Colour</label>
                            <asp:TextBox runat="server" ID="txtNameColour" CssClass="form-control" placeholder="Colour ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-4">
                            <label class="form-label required">Width</label>
                            <asp:TextBox runat="server" ID="txtColourWidth" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-3">
                            <label class="form-label required">Active</label>
                            <asp:DropDownList runat="server" ID="ddlColourActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorAddColour">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorAddColour"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancelAddColour" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitAddColour" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitAddColour_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDelete" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Fabric</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col">
                                <asp:Button runat="server" ID="btnCancelDelete" Text="Cancel" CssClass="btn w-100" OnClick="btnCancel_Click" />
                            </div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitDelete" CssClass="btn btn-danger w-100" Text="Confirm" OnClick="btnSubmitDelete_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeleteColour" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Fabric Colour</h3>
                    <asp:TextBox runat="server" ID="txtIdColourDelete" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col">
                                <asp:Button runat="server" ID="btnCancelDeleteColour" Text="Cancel" CssClass="btn w-100" OnClick="btnCancel_Click" />
                            </div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitDeleteColour" CssClass="btn btn-danger w-100" Text="Confirm" OnClick="btnSubmitDeleteColour_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Changelog</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorLog">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorLog"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvListLogs" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="DATA LOG NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeader="false" GridLines="None" BorderStyle="None">
                            <RowStyle />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%# BindTextLog(Eval("Id").ToString()) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle BackColor="White" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showDeleteColour(id) {
            document.getElementById('<%=txtIdColourDelete.ClientID %>').value = id;
        }
        function showEdit() {
            $('#modalEdit').modal('show');
        }
        function showAddColour() {
            $('#modalAddColour').modal('show');
        }
        function showLog() {
            $("#modalLog").modal("show");
        }
        ["modalEdit", "modalAddColour", "modalDelete", "modalDeleteColour"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        document.addEventListener("DOMContentLoaded", function () {
            var el;
            window.TomSelect && (new TomSelect(el = document.getElementById('lbDesign'), {
                copyClassesToDropdown: false,
                dropdownParent: document.getElementById('#modalEdit'),
                controlInput: '<input>',
                render: {
                    item: function (data, escape) {
                        if (data.customProperties) {
                            return '<div><span class="dropdown-item-indicator">' + data.customProperties + '</span>' + escape(data.text) + '</div>';
                        }
                        return '<div>' + escape(data.text) + '</div>';
                    },
                    option: function (data, escape) {
                        if (data.customProperties) {
                            return '<div><span class="dropdown-item-indicator">' + data.customProperties + '</span>' + escape(data.text) + '</div>';
                        }
                        return '<div>' + escape(data.text) + '</div>';
                    },
                },
            }));
        });

        document.addEventListener("DOMContentLoaded", function () {
            var el;
            window.TomSelect && (new TomSelect(el = document.getElementById('lbTube'), {
                copyClassesToDropdown: false,
                dropdownParent: document.getElementById('#modalEdit'),
                controlInput: '<input>',
                render: {
                    item: function (data, escape) {
                        if (data.customProperties) {
                            return '<div><span class="dropdown-item-indicator">' + data.customProperties + '</span>' + escape(data.text) + '</div>';
                        }
                        return '<div>' + escape(data.text) + '</div>';
                    },
                    option: function (data, escape) {
                        if (data.customProperties) {
                            return '<div><span class="dropdown-item-indicator">' + data.customProperties + '</span>' + escape(data.text) + '</div>';
                        }
                        return '<div>' + escape(data.text) + '</div>';
                    },
                },
            }));
        });
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblActionColour" ></asp:Label>
        <asp:Label runat="server" ID="lblIdColour" ></asp:Label>
    </div>
</asp:Content>
