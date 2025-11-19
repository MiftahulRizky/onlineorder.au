<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Product_Detail" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.Master" Debug="true" Title="Product Detail" %>

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
                        <a runat="server" href="~/setting/product" class="text-decoration-none">Product</a>
                    </h2>
                </div>

                <div class="col-6 text-end">
                    <asp:Button runat="server" ID="btnEdit" CssClass="btn btn-primary" Text="Edit" OnClick="btnEdit_Click" />
                    <a href="#" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete</a>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row mb-3" runat="server" id="divError">
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
                            <h3 class="card-title">Detail Product</h3>
                        </div>

                        <div class="card-body">
                            <div class="datagrid">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">DESIGN TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblDesignName"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">BLIND TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblBlindName"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">PRODUCT NAME</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblName"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">TUBE TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblTubeName"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">CONTROL Type</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblControlName"></asp:Label>
                                    </div>
                                </div>
                                
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">COLOUR TYPE</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblColourName"></asp:Label>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">DESCRITPION</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:Label runat="server" ID="lblDescription"></asp:Label>
                                    </div>
                                </div>
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">ACTIVE</div>
                                    <div class="datagrid-content">
                                        <div runat="server" id="divActiveYes">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-square-rounded-check text-green">
                                              <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                                              <path d="M9 12l2 2l4 -4" />
                                              <path d="M12 3c7.2 0 9 1.8 9 9s-1.8 9 -9 9s-9 -1.8 -9 -9s1.8 -9 9 -9z" />
                                            </svg>
                                            Yes
                                        </div>
                                        <div runat="server" id="divActiveNo">
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
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Data HardwareKit</h3>
                            <div class="card-actions">
                                <asp:Button runat="server" ID="btnAddKit" CssClass="btn btn-orange" Text="Add New" OnClick="btnAddKit_Click" />
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="KIT DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvList_RowCommand">
                                    <RowStyle />
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                        <asp:BoundField DataField="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                        <asp:BoundField DataField="Name" HeaderText="Name" />
                                        <asp:BoundField DataField="KitId" HeaderText="Kit" />
                                        <asp:BoundField DataField="VenId" HeaderText="Ven" />
                                        <asp:BoundField DataField="BlindStatus" HeaderText="Blind Status" />
                                        <asp:TemplateField ItemStyle-Width="180px">
                                            <ItemTemplate>
                                                <button class="btn btn-sm btn-pill btn-primary" data-bs-toggle="dropdown">Actions</button>
                                                <div class="dropdown-menu dropdown-menu-end">
                                                    <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                    <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteKit" onclick='<%# String.Format("return showDeleteKit(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                    <div class="dropdown-divider"></div>
                                                    <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkLog" Text="Logs" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>

                        <div class="card-footer text-end"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Product</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitDelete" CssClass="btn btn-danger w-100" Text="Confirm" OnClick="btnSubmitDelete_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleProcess"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <div class="col-lg-6 col-md-12 col-sm-12">
                            <label class="form-label">Kit ID :</label>
                            <asp:TextBox runat="server" ID="txtKitId" TextMode="Number" CssClass="form-control" placeholder="Kit Id ..." autocomplete="off" Text="0"></asp:TextBox>
                        </div>

                        <div class="col-lg-6 col-md-12 col-sm-12">
                            <label class="form-label">Ven ID :</label>
                            <asp:TextBox runat="server" ID="txtVenId" TextMode="Number" CssClass="form-control" placeholder="Ven Id ..." autocomplete="off" Text="0"></asp:TextBox>
                        </div>
                    </div>
                
                    <div class="mb-3 row">
                        <label class="form-label">Name :</label>
                        <div class="col-12">
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtKitName" Height="70px" CssClass="form-control" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                
                    <div class="mb-3 row" runat="server" id="divCustomName">
                        <label class="form-label">Custom Name :</label>
                        <div class="col-12">
                            <asp:TextBox runat="server" ID="txtCutomKitName" CssClass="form-control" placeholder="Custom Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                
                    <div class="mb-3 row">
                        <label class="form-label">Blind Status :</label>
                        <div class="col-lg-4 col-md-12 col-sm-12">
                            <asp:DropDownList runat="server" ID="ddlBlindStatus" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Control" Text="CONTROL"></asp:ListItem>
                                <asp:ListItem Value="Middle" Text="MIDDLE"></asp:ListItem>
                                <asp:ListItem Value="End" Text="END"></asp:ListItem>
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Metal" Text="METAL"></asp:ListItem>
                                <asp:ListItem Value="Semi Metal" Text="SEMI METAL"></asp:ListItem>
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="LD" Text="LD"></asp:ListItem>
                                <asp:ListItem Value="LD SA" Text="LD SA"></asp:ListItem>
                                <asp:ListItem Value="HD" Text="HD"></asp:ListItem>
                                <asp:ListItem Value="HD SA" Text="HD SA"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorProcess"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn" data-bs-dismiss="modal">Close</button>
                    <asp:Button runat="server" ID="btnSubmitProcess" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitProcess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeleteKit" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Hardware Kit</h3>
                    <asp:TextBox runat="server" ID="txtIdKitDelete" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitDeleteKit" CssClass="btn btn-danger w-100" Text="Confirm" OnClick="btnSubmitDeleteKit_Click" />
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
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
        function showDeleteKit(id) {
            document.getElementById('<%=txtIdKitDelete.ClientID %>').value = id;
        }
        function showProcess() {
            $('#modalProcess').modal('show');
        }
        function showLog() {
            $("#modalLog").modal("show");
        }
    </script>
    
    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblIdKit"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>
</asp:Content>
