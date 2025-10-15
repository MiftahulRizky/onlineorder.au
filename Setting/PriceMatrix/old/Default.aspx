<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_PriceMatrix_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Price Matrix" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Price Matrix</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Data Matrix</h3>
                            <div class="card-actions">
                                <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasFilter" role="button" aria-controls="canvasFilter">
                                    <i class="fa-solid fa-filter" style="margin-right: 0.3rem;"></i> Filter Matrix
                                </a>

                                <button class="btn btn-info dropdown-toggle " data-bs-toggle="dropdown">
                                    <i class="fa-solid fa-plus" style="margin-right: 0.3rem;"></i> Add Matrix
                                </button>
                                <div class="dropdown-menu dropdown-menu-end">
                                    <asp:Button runat="server" ID="btnAdd" CssClass="dropdown-item" Text="Add New Line" OnClick="btnAdd_Click" />
                                    <asp:Button runat="server" ID="btnImport" CssClass="dropdown-item" Text="Import CSV" OnClick="btnImport_Click" />
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
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="MATRIX DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" PageSize="50" OnPageIndexChanging="gvList_PageIndexChanging">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="NO">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="GroupName" HeaderText="GROUP NAME" />
                                    <asp:BoundField DataField="Type" HeaderText="TYPE" />
                                    <asp:BoundField DataField="Width" HeaderText="WIDTH" />
                                    <asp:BoundField DataField="Drop" HeaderText="DROP" />
                                    <asp:BoundField DataField="Cost" HeaderText="COST" />
                                    <asp:TemplateField HeaderText="ACTION">
                                        <ItemTemplate>
                                            <div class="float-end">
                                                <%-- <a title="Edit" class="btn btn-primary" data-bs-toggle="offcanvas" href="#canvasEdit" role="button" aria-controls="canvasEdit" onclick='<%# String.Format("return DetailData(`{0}`, `{1}`, `{2}`, `{3}`, `{4}`);", Eval("Id").ToString(), Eval("GroupName").ToString(), Eval("Width").ToString(), Eval("Drop").ToString(), Eval("Cost")) %>' style="height: 2.3em; width: 2em;"><i class="fa-regular fa-pen-to-square"></i></a>
                                                
                                                <asp:LinkButton runat="server" title="Delete" ID="linkDelete" CssClass="btn btn-danger" Text="Delete" OnClick="linkDelete_Click" style="height: 2.3em; width: 2em;">
                                                    <i class="fa-regular fa-trash-can"></i>
                                                </asp:LinkButton> --%>


                                                <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
                                                </button>
                                                <ul class="dropdown-menu dropdown-menu-end">
                                                    <li>
                                                        <a class="dropdown-item" data-bs-toggle="offcanvas" href="#canvasEdit" role="button" aria-controls="canvasEdit" onclick='<%# String.Format("return DetailData(`{0}`, `{1}`, `{2}`, `{3}`, `{4}`);", Eval("Id").ToString(), Eval("GroupName").ToString(), Eval("Width").ToString(), Eval("Drop").ToString(), Eval("Cost")) %>'>
                                                            <i class="bi bi-pencil-square me-2 opacity-50"></i>Edit
                                                        </a>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton runat="server" ID="linkDelete" CssClass="dropdown-item text-danger" OnClick="linkDelete_Click">
                                                            <i class="bi bi-trash3 me-2"></i>Delete
                                                        </asp:LinkButton>
                                                    </li>
                                                </ul>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="offcanvas offcanvas-end" tabindex="-1" id="canvasFilter" aria-labelledby="canvasFilterLabel">
        <div class="offcanvas-header">
            <h2 class="offcanvas-title" id="canvasShortLabel">Filter Matrix</h2>
            <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
            <div class="mb-3 row">
                <label class="form-label">PRICE GROUP</label>
                <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="form-select price-group"></asp:DropDownList>
            </div>

            <div class="mb-3 row">
                <label class="form-label">PRICE LEVEL</label>
                <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                    <asp:ListItem Value="" Text=""></asp:ListItem>
                    <asp:ListItem Value="Pick Up" Text="PICK UP"></asp:ListItem>
                    <asp:ListItem Value="Delivery" Text="DELIVERY"></asp:ListItem>
                    <asp:ListItem Value="INT-FIS" Text="INT-FIS"></asp:ListItem>
                    <asp:ListItem Value="INT-PU" Text="INT-PU"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <div class="mb-3 row">
                <label class="form-label">WIDTH</label>
                <asp:TextBox runat="server" ID="txtWidth" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
            </div>

            <div class="mb-3 row">
                <label class="form-label">DROP</label>
                <asp:TextBox runat="server" ID="txtDrop" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
            </div>

            <div class="mt-3">
                <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
            </div>
        </div>
    </div>

    <div class="offcanvas offcanvas-end" tabindex="-1" id="canvasEdit" aria-labelledby="canvasEditLabel">
        <div class="offcanvas-header">
            <h2 class="offcanvas-title" id="canvasEditLabel">Update Matrix Price</h2>
            <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
            <asp:TextBox runat="server" ID="txtPriceIdEdit" CssClass="form-control" placeholder="Name ..." autocomplete="off" style="display:none;"></asp:TextBox>

            <div class="mb-5 row">
                <div class="col-12">
                    <label class="form-label required">1. GROUP NAME</label>
                    <asp:TextBox runat="server" ID="txtGroupName" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
            </div>

            <div class="mb-5 row">
                <div class="col-6">
                    <label class="form-label required">2. WIDTH</label>
                    <asp:TextBox runat="server" ID="txtWidthEdit" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>

                <div class="col-6">
                    <label class="form-label">3. DROP</label>
                    <asp:TextBox runat="server" ID="txtDropEdit" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
            </div>

            <div class="mb-5 row">
                <div class="col-12">
                    <label class="form-label required">4. COST</label>
                    <asp:TextBox runat="server" ID="txtCostEdit" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                </div>
            </div>

            <div class="mt-3">
                <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
            </div>
        </div>
    </div>
    <!-- <script text="text/javascript" src="/Scripts/SettingPage/DefaultPriceMatrix.js"></script> -->
    <script type="text/javascript">
        function DetailData(Id, GroupName, Width, Drop, Cost) {
            document.getElementById('<%=txtPriceIdEdit.ClientID %>').value = Id;
            document.getElementById('<%=txtGroupName.ClientID %>').value = GroupName;
            document.getElementById('<%=txtWidthEdit.ClientID %>').value = Width;
            document.getElementById('<%=txtDropEdit.ClientID %>').value = Drop;
            document.getElementById('<%=txtCostEdit.ClientID %>').value = Cost;
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE Prices SET [Cost]=@Cost WHERE Id=@Id" DeleteCommand="DELETE FROM Prices WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCostEdit" Name="Cost" PropertyName="Text" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>