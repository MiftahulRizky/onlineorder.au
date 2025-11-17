<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Discount.aspx.vb" Inherits="Setting_Customer_Discount" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer Discount" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col-6">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Customer Discount</h2>
                </div>
                <div class="col-6 text-end">
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-12">
                    <div class="card" id="dvTab">
                        <div class="card-header">
                            <ul class="nav nav-tabs card-header-tabs nav-fill" data-bs-toggle="tabs">
                                <li class="nav-item">
                                    <a href="#tabsProducts" class="nav-link active" data-bs-toggle="tab">PRODUCTS</a>
                                </li>
                                <li class="nav-item">
                                    <a href="#tabsFabrics" class="nav-link" data-bs-toggle="tab">FABRICS</a>
                                </li>
                            </ul>
                        </div>

                        <div class="card-body">
                            <div class="tab-content">
                                <div class="tab-pane active show" id="tabsProducts">
                                    <div class="row mb-3" runat="server" id="divErrorProducts">
                                        <div class="col-12">
                                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                                <div class="d-flex">
                                                    <div>
                                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                                    </div>
                                                    <div>
                                                        <span runat="server" id="msgErrorProducts"></span>
                                                    </div>
                                                </div>
                                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListProducts" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="#" ItemStyle-Width="80px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="DISCOUNT" ItemStyle-Width="200px">
                                                            <ItemTemplate>
                                                                <%# DiscountDataProduct(Eval("DesignId").ToString(), Eval("Discount")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="CUSTOMER">
                                                            <ItemTemplate>
                                                                <%# DiscountCustomerProduct(Eval("DesignId").ToString(), Eval("Discount"), Eval("CustomerBy").ToString()) %>
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

                                <div class="tab-pane" id="tabsFabrics">
                                    <div class="row mb-3" runat="server" id="divErrorFabrics">
                                        <div class="col-12">
                                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                                <div class="d-flex">
                                                    <div>
                                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                                    </div>
                                                    <div>
                                                        <span runat="server" id="msgErrorFabrics"></span>
                                                    </div>
                                                </div>
                                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvLlistFabrics" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField HeaderText="#" ItemStyle-Width="80px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Customer Name">
                                                            <ItemTemplate>
                                                                <%# DiscountCustomerFabric(Eval("CustomerBy").ToString(), Eval("CustomerData").ToString()) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="FabricName" HeaderText="Fabric Name" />
                                                        <asp:TemplateField HeaderText="Discount">
                                                            <ItemTemplate>
                                                                <%# TeksDiscount(Eval("Discount")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:dd MMM yyyy}" />
                                                        <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:dd MMM yyyy}" />
                                                        <asp:BoundField DataField="DiscFinal" HeaderText="Final Discount" />
                                                        <asp:BoundField DataField="DiscActive" HeaderText="Active" />
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
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="selected_tab" runat="server"  />

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', () => {
        });
        
        $(document).ready(function () {
            loaderFadeOut();
            
            var selectedTab = $("#<%=selected_tab.ClientID%>");
            var tabId = selectedTab.val() != "" ? selectedTab.val() : "tabsProducts";
            $('#dvTab a[href="#' + tabId + '"]').tab('show');
            $("#dvTab a").click(function () {
                selectedTab.val($(this).attr("href").substring(1));
            });
        });
    </script>
</asp:Content>

