<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Customer_Group_Discount_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer Group Discount" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Discount Customer Group</h2>
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
                            <h3 class="card-title" runat="server" id="hTitle">Add Customer Group</h3>
                            <div class="card-actions">
                                <asp:Button runat="server" ID="btnFinish" CssClass="btn btn-success" Text="Finish" OnClick="btnFinish_Click" />
                            </div>
                        </div>

                        <div class="card-body">
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

                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DISCOUNT DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                    <RowStyle />
                                    <Columns>
                                        <asp:TemplateField HeaderText="#">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                        <asp:TemplateField HeaderText="Title">
                                            <ItemTemplate>
                                                <%# TextType(Eval("Id").ToString()) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Discount">
                                            <ItemTemplate>
                                                <%# ValueDiscount(Eval("Discount")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:MMM dd, yyyy}" />
                                        <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:MMM dd, yyyy}" />
                                        <asp:TemplateField HeaderText="Final Discount (Fabric)">
                                            <ItemTemplate>
                                                <%# FinalDiscount(Eval("DiscountType"), Eval("FinalDiscount")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="btn btn-sm btn-primary btn-pill" Text="Detail / Edit"></asp:LinkButton>
                                                <a href="#" runat="server" class="btn btn-sm btn-danger btn-pill" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                    <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                    <AlternatingRowStyle BackColor="" />
                                </asp:GridView>
                            </div>
                        </div>
                        <div class="card-footer text-start">
                            <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add Discount" OnClick="btnAdd_Click" />
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
                    <h3>Delete Customer Discount</h3>
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
        });
        function showDelete(id) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
        }
    </script>
    

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblIdDiscount"></asp:Label>
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblCustomerDiscount"></asp:Label>
        <asp:Label runat="server" ID="lblDiscountType"></asp:Label>
        <asp:Label runat="server" ID="lblFabricCustom"></asp:Label>
        <asp:Label runat="server" ID="lblFabricColourId"></asp:Label>
        <asp:Label runat="server" ID="lblStartDate"></asp:Label>
        <asp:Label runat="server" ID="lblEndDate"></asp:Label>

        <asp:Label runat="server" ID="lblAction"></asp:Label>

        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO CustomerDiscounts VALUES (@Id, GETDATE(), GETDATE(), 'CustomerGroup', @CustomerId, @DiscountType, @CustomFabric, @DesignId, @FabricId, @FabricColourId, @Discount, @StartDate, @EndDate, @FinalDiscount, 1)" UpdateCommand="UPDATE CustomerDiscounts SET Discount=@Discount, StartDate=@StartDate, EndDate=@EndDate, FinalDiscount=@FinalDiscount WHERE Id=@Id" DeleteCommand="UPDATE CustomerDiscounts SET Active=0 WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblCustomerDiscount" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblId" Name="CustomerId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblDiscountType" Name="DiscountType" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblFabricCustom" Name="CustomFabric" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlDesignType" Name="DesignId" PropertyName="SelectedItem.Value" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="ddlFabricType" Name="FabricId" PropertyName="SelectedItem.Value" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="lblFabricColourId" Name="FabricColourId" PropertyName="Text" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="txtDiscount" Name="Discount" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblStartDate" Name="StartDate" PropertyName="Text" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="lblEndDate" Name="EndDate" PropertyName="Text" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="ddlDiscountFinal" Name="FinalDiscount" PropertyName="SelectedItem.Value" />
            </InsertParameters>
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblCustomerDiscount" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblId" Name="CustomerId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblDiscountType" Name="DiscountType" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblFabricCustom" Name="CustomFabric" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlDesignType" Name="DesignId" PropertyName="SelectedItem.Value" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="ddlFabricType" Name="FabricId" PropertyName="SelectedItem.Value" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="lblFabricColourId" Name="FabricColourId" PropertyName="Text" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="txtDiscount" Name="Discount" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblStartDate" Name="StartDate" PropertyName="Text" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="lblEndDate" Name="EndDate" PropertyName="Text" ConvertEmptyStringToNull="true" />
                <asp:ControlParameter ControlID="ddlDiscountFinal" Name="FinalDiscount" PropertyName="SelectedItem.Value" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblCustomerDiscount" Name="Id" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>