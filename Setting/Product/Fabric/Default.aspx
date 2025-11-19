<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Product_Fabric_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Fabric" %>

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
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Data Fabric</h3>
                            <div class="card-actions">
                                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add New" OnClick="btnAdd_Click" />
                            </div>
                        </div>

                        <div class="card-body border-bottom py-3">
                            <div class="d-flex">
                                <div class="ms-auto text-secondary">
                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                        <div class="ms-2 d-inline-block">
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholder="Search Data" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="ms-2 d-inline-block">
                                            <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                        </div>
                                    </asp:Panel>
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
                        
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="FABRIC DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="Name" HeaderText="Name" />
                                    <asp:BoundField DataField="Factory" HeaderText="Factory" ItemStyle-Width="100px" />
                                    <asp:BoundField DataField="Group" HeaderText="Group" />
                                    <asp:TemplateField HeaderText="Product">
                                        <ItemTemplate>
                                            <%# BindDetailProduct(Eval("Id").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="TubeData" HeaderText="Tube" />
                                    <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px">
                                        <ItemTemplate>
                                            <button class="btn btn-sm btn-pill btn-orange" data-bs-toggle="dropdown">Actions</button>
                                            <div class="dropdown-menu dropdown-menu-end">
                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>' visible='<%# VisibleAction() %>'>Delete</a>
                                                <div class="dropdown-divider"></div>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkLog" Text="Logs" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>
                        <div class="card-footer text-start"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalAdd" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-full-width modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Fabric</h5>
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

                    <div class="row" runat="server" id="divErrorAdd">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorAdd"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancelAdd" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitAdd" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitAdd_Click" />
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
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:Button runat="server" ID="btnCancelDelete" Text="Cancel" CssClass="btn" OnClick="btnCancel_Click" />
                    <asp:Button runat="server" ID="btnSubmitDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnSubmitDelete_Click" />
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
        function showAdd() {
            $("#modalAdd").modal("show");
        }
        function showDelete(id) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
        }
        function showLog() {
            $("#modalLog").modal("show");
        }
        ["modalAdd", "modalDelete", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        document.addEventListener("DOMContentLoaded", function () {
            function initializeTomSelect(elementId) {
                var el = document.getElementById(elementId);
                if (window.TomSelect && el) {
                    new TomSelect(el, {
                        copyClassesToDropdown: false,
                        dropdownParent: document.getElementById("#modalAdd"),
                        controlInput: "<input>",
                        render: {
                            item: renderItem,
                            option: renderItem
                        }
                    });
                }
            }

            function renderItem(data, escape) {
                return data.customProperties
                    ? `<div><span class="dropdown-item-indicator">${data.customProperties}</span>${escape(data.text)}</div>`
                    : `<div>${escape(data.text)}</div>`;
            }

            initializeTomSelect("lbDesign");
            initializeTomSelect("lbTube");
        });
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId" ></asp:Label>
    </div>
</asp:Content>