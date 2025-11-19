<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Matrix.aspx.vb" Inherits="Setting_Price_Matrix" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Price Matrix" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .datagridTitle{ font-size:14px; }
        .datagridContent { font-size:16px; }
    </style>
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col-4">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Price Matrix</h2>
                </div>
                <div class="col-8 text-end">
                    <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-info" Text="Add Matrix" OnClick="btnAdd_Click" />
                    <asp:Button runat="server" ID="btnImport" CssClass="btn btn-secondary" Text="Import Matrix" OnClick="btnImport_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row mt-3" runat="server" id="divError">
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
                            <h3 class="card-title">Data Price Matrix</h3>
                        </div>

                        <div class="card-body">
                            <div class="datagrid">
                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Price Group Name</div>
                                    <div class="datagrid-content datagridContent">
                                        <asp:DropDownList runat="server" ID="ddlGroupSearch" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Width</div>
                                    <div class="datagrid-content datagridContent">
                                        <div class="input-group">
                                            <asp:TextBox runat="server" ID="txtWidthSearch" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                            <span class="input-group-text">mm</span>
                                        </div>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title datagridTitle">Drop / Height</div>
                                    <div class="datagrid-content datagridContent">
                                        <div class="input-group">
                                            <asp:TextBox runat="server" ID="txtDropSearch" TextMode="Number" CssClass="form-control" placeholder="Drop / Height ..." autocomplete="off"></asp:TextBox>
                                            <span class="input-group-text">mm</span>
                                        </div>
                                    </div>
                                </div>

                                <div class="datagrid-item">
                                    <div class="datagrid-title">&nbsp;</div>
                                    <div class="datagrid-content">
                                        <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>

                        <hr />

                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="MATRIX DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" PageSize="50" OnPageIndexChanging="gvList_PageIndexChanging">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="GroupName" HeaderText="GROUP" ReadOnly="true" />
                                    <asp:BoundField DataField="Width" HeaderText="WIDTH" ReadOnly="true" />
                                    <asp:BoundField DataField="Drop" HeaderText="DROP" ReadOnly="true" />
                                    <asp:TemplateField HeaderText="COST">
                                        <ItemTemplate>
                                            <%#BindCost(Eval("Cost")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="200px">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="linkEdit" CssClass="btn btn-sm btn-pill btn-info" Text="Edit" OnClick="linkEdit_Click" Visible='<%# VisibleEdit() %>'></asp:LinkButton>
                                            <a href="#" runat="server" class="btn btn-sm btn-pill btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>' visible='<%# VisibleDelete() %>'>Delete</a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>

                        <div class="card-footer"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalProccess" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleProccess"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="mb-5 row">
                        <div class="col-12">
                            <label class="form-label required">Price Group Name</label>
                            <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-5 row">
                        <div class="col-6">
                            <label class="form-label required">Width</label>
                            <div class="input-group">
                                <asp:TextBox runat="server" ID="txtWidth" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                <span class="input-group-text">mm</span>
                            </div>
                        </div>

                        <div class="col-6">
                            <label class="form-label required">Drop</label>
                            <div class="input-group">
                                <asp:TextBox runat="server" ID="txtDrop" TextMode="Number" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                <span class="input-group-text">mm</span>
                            </div>
                        </div>
                    </div>

                    <div class="mb-5 row">
                        <div class="col-5">
                            <label class="form-label required">Cost</label>
                            <div class="input-group">
                                <span class="input-group-text">$</span>
                                <asp:TextBox runat="server" ID="txtCost" CssClass="form-control" placeholder="Cost ..." autocomplete="off"></asp:TextBox>
                            </div>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorProccess">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorProccess"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitProccess" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitProccess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalImport" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Import Matrix</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="mb-3 row">
                        <div class="col-12">
                            <label class="form-label required">FILE TO IMPORT</label>
                            <asp:FileUpload runat="server" ID="fuFile" CssClass="form-control" />
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorImport">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorImport"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitImport" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmitImport_Click" />
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
                    <h3>Delete Matrix</h3>
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
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

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', () => {
            loaderFadeOut();
        })
        function showDelete(id, groupname, width, drop) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
        }
        function showProccess() {
            $("#modalProccess").modal("show");
        }
        function showImport() {
            $("#modalImport").modal("show");
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblPriceGroupId"></asp:Label>
        <asp:Label runat="server" ID="lblCost"></asp:Label>

        <asp:Label runat="server" ID="lblAction"></asp:Label>

        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO PriceMatrixs VALUES (NEWID(), @PriceGroup, @Drop, @Width, @Cost, GETDATE(), NULL)" UpdateCommand="UPDATE PriceMatrixs SET Width=@Width, [Drop]=@Drop, Cost=@Cost, UpdatedDate=GETDATE() WHERE Id=@Id" DeleteCommand="DELETE FROM PriceMatrixs WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroup" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCost" Name="Cost" PropertyName="Text" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblPriceGroupId" Name="PriceGroup" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDrop" Name="Drop" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtWidth" Name="Width" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCost" Name="Cost" PropertyName="Text" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>