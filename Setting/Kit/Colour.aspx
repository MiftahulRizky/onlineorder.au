<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Colour.aspx.vb" Inherits="Setting_Kit_Colour" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Colour Type" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Colour Type</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-9 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Data Colour Type</h3>
                            <div class="card-actions">
                                <a class="btn btn-primary " data-bs-toggle="offcanvas" href="#canvasAdd" role="button" aria-controls="canvasAdd">
                                    <i class="fa-solid fa-plus me-2"></i>Add New
                                </a>
                                <asp:LinkButton runat="server" ID="btnKit" CssClass="btn btn-success " Text="Kit List" OnClick="btnKit_Click" >
                                    <i class="bi bi-card-checklist fs-3 me-2"></i>Kit List
                                </asp:LinkButton>
                            </div>
                        </div>

                        <div class="card-body border-bottom py-3">
                            <div class="d-flex">
                                <div class="ms-auto text-secondary">
                                    <asp:Panel runat="server" DefaultButton="btnSearch">
                                        <div class="ms-2 d-inline-block">
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control " placeholder="Search Data" autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="ms-2 d-inline-block">
                                            <asp:LinkButton runat="server" ID="btnSearch" CssClass="btn btn-primary " Text="Search" OnClick="btnSearch_Click" >
                                                <i class="fa-solid fa-magnifying-glass me-2"></i>Search
                                            </asp:LinkButton>
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
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="CONTROL TYPE DATA NOT FOUND" PageSize="15" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="70px">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:TemplateField HeaderText="Name">
                                        <ItemTemplate>
                                            <%# IconActiveOnNames(Eval("Active").ToString(), Eval("Name").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Active" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />

                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="200px">
                                        <ItemTemplate>
                                            <div class="float-end">
                                                <%-- <a class="btn btn-sm btn-primary" data-bs-toggle="offcanvas" href="#canvasEdit" role="button" aria-controls="canvasEdit" onclick='<%# String.Format("return DetailData(`{0}`, `{1}`, `{2}`);", Eval("Id"), Eval("Name"), Eval("Description")) %>'>Edit</a>

                                                <a href="#" runat="server" class="btn btn-sm btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Name").ToString()) %>'> Delete </a>

                                                <asp:LinkButton runat="server" ID="linkActive" CssClass='<%# CssActive(Eval("Active")) %>' Text='<%# TextActive(Eval("Active").ToString()) %>' OnClick="linkActive_Click"></asp:LinkButton> --%>
                                                
                                                
                                                <!-- New Action Button  -->
                                                <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
                                                </button>
                                                <ul class="dropdown-menu dropdown-menu-end">
                                                  <li>
                                                    <a class="dropdown-item" data-bs-toggle="offcanvas" href="#canvasEdit" role="button" aria-controls="canvasEdit" onclick='<%# String.Format("return DetailData(`{0}`, `{1}`, `{2}`);", Eval("Id"), Eval("Name"), Eval("Description")) %>'>
                                                        <i class="bi bi-pencil-square me-2 opacity-50"></i>Edit
                                                    </a>
                                                  </li>
                                                  <li>
                                                    <a href="javascript:void(0);" runat="server" class="dropdown-item text-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Name").ToString()) %>'>
                                                        <i class="bi bi-trash3 me-2"></i>Delete
                                                    </a>
                                                  </li>
                                                  <li>
                                                    <asp:LinkButton runat="server" ID="linkActive" CssClass='<%# CssActive(Eval("Active")) %>' Text='<%# TextActive(Eval("Active").ToString()) %>' OnClick="linkActive_Click"></asp:LinkButton>
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

    <div class="modal modal-blur fade" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Data?</h3>
                    <div class="text-secondary">
                        <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                        <span id="spanDescription"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100 " data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger w-100 " Text="Yes. Delete" OnClick="btnDelete_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="offcanvas offcanvas-end" tabindex="-1" id="canvasAdd" aria-labelledby="canvasAddLabel">
        <div class="offcanvas-header">
            <h2 class="offcanvas-title" id="canvasAddLabel">Add Colour Type</h2>
            <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
            <div class="mb-5 row">
                <div class="col-12">
                    <label class="form-label required">1. NAME</label>
                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                </div>
            </div>

            <div class="mb-5 row">
                <div class="col-12">
                    <label class="form-label">2. DESCRIPTION</label>
                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                </div>
            </div>

            <div class="mb-5 row">
                <div class="col-12">
                    <label class="form-label required">3. ACTIVE</label>
                    <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                        <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                        <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>

            <div class="mt-3">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Submit" OnClick="btnAdd_Click" />
            </div>
        </div>
    </div>

    <div class="offcanvas offcanvas-end" tabindex="-1" id="canvasEdit" aria-labelledby="canvasEditLabel">
        <div class="offcanvas-header">
            <h2 class="offcanvas-title" id="canvasEditLabel">Edit Colour Type</h2>
            <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
            <div class="mb-5 row">
                <asp:TextBox runat="server" ID="txtIdEdit" style="display:none;"></asp:TextBox>
                <div class="col-12">
                    <label class="form-label required">1. NAME</label>
                    <asp:TextBox runat="server" ID="txtNameEdit" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                </div>
            </div>

            <div class="mb-5 row">
                <div class="col-12">
                    <label class="form-label">2. DESCRIPTION</label>
                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescriptionEdit" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                </div>
            </div>

            <div class="mt-3">
                <asp:Button runat="server" ID="btnUpdate" CssClass="btn btn-primary" Text="Submit" OnClick="btnUpdate_Click" />
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showDelete(id, name) {
            var result = 'Are you sure you want to delete this colour type?';
            result += '<br /><br />';
            result += '<b>DATA : ' + name.toUpperCase() + '</b>';
            document.getElementById('<%=txtIdDelete.ClientID %>').value = id;
            document.getElementById("spanDescription").innerHTML = result;
        }

        function DetailData(Id, Name, Description) {            
            document.getElementById('<%=txtIdEdit.ClientID %>').value = Id;
            document.getElementById('<%=txtNameEdit.ClientID %>').value = Name;
            document.getElementById('<%=txtDescriptionEdit.ClientID %>').value = Description;
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblActive"></asp:Label>

        <asp:SqlDataSource ID="sdsAddUpdate" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO ColourType VALUES (NEWID(), @Name, @Description, @Active)" UpdateCommand="UPDATE ColourType SET Name=@Name, Description=@Description, Active=@Active WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtNameEdit" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDescriptionEdit" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActiveEdit" Name="Active" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" DeleteCommand="DELETE FROM ColourType WHERE Id=@Id" UpdateCommand="UPDATE ColourType SET Active=@Active WHERE Id=@Id">
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblActive" Name="Active" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
    </script>
</asp:Content>