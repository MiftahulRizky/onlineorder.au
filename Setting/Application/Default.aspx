<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Application_Default" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Application" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Application</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row mb-3">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Data Application</h3>
                            <div class="card-actions">
                                <asp:LinkButton runat="server" ID="btnAdd" CssClass="btn btn-primary " Text="Add Application" OnClick="btnAdd_Click" >
                                    <i class="fa-solid fa-plus" style="margin-right: 0.3rem;"></i> Add Application
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
                                                <i class="fa-solid fa-magnifying-glass" style="margin-right: 0.3rem;"></i> Search
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
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="True" Position="TopAndBottom" EmptyDataText="APPLICATION DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="50" OnPageIndexChanging="gvList_PageIndexChanging">
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#" ItemStyle-Width="80px">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:TemplateField HeaderText="NAME">
                                        <ItemTemplate>
                                            <%# IconActiveOnNames(Eval("Active").ToString(), Eval("Name").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Description" HeaderText="DESCRIPTION" />
                                    <asp:BoundField DataField="Active" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="200px">
                                        <ItemTemplate>
                                            <div class="float-end">
                                                <%-- <asp:LinkButton runat="server" title="Detail" ID="linkDetail" CssClass="btn btn-primary" Text="Detail" OnClick="linkDetail_Click" style="height: 2.3em; width: 2em;">
                                                    <i class="fa-regular fa-eye"></i>
                                                </asp:LinkButton>
                                                
                                                <a href="#" title="Delete" runat="server" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Name").ToString()) %>' style="height: 2.3em; width: 2em;">
                                                    <i class="fa-regular fa-trash-can"></i>
                                                </a>
                                                
                                                <asp:LinkButton runat="server" ID="linkActive" CssClass='<%# CssActive(Eval("Active")) %>' Text='<%# TextActive(Eval("Active").ToString()) %>' OnClick="linkActive_Click" style="height: 2.3em; width: 2em;"></asp:LinkButton> --%>


                                                <button class="border-0 bg-transparent dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <i class="bi bi-three-dots-vertical fs-1 opacity-50"></i>
                                                </button>
                                                <ul class="dropdown-menu dropdown-menu-end">
                                                    <li>
                                                        <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" OnClick="linkDetail_Click">
                                                            <i class="bi bi-info-circle me-2 opacity-50"></i>Detail
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <a href="javascript:void(0)" runat="server" class="dropdown-item text-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Name").ToString()) %>'>
                                                            <i class="bi bi-trash3 me-2 "></i>Delete
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
                    <h3>Are you sure?</h3>
                    <div class="text-secondary">
                        <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                        <span id="spanDescription"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger w-100" Text="Yes. Delete" OnClick="btnDelete_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
        function showDelete(id, name) {
            var result = 'Are you sure you want to delete this application?';
            result += '<br />';
            result += '<b>' + name.toUpperCase() + '</b>';
            document.getElementById('<%=txtIdDelete.ClientID %>').value = id;
            document.getElementById("spanDescription").innerHTML = result;
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblActive"></asp:Label>

        <asp:SqlDataSource ID="sdsApp" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" DeleteCommand="DELETE FROM Applications WHERE Id=@Id" UpdateCommand="UPDATE Applications SET Active=@Active WHERE Id=@Id">
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
            </DeleteParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblActive" Name="Active" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>
