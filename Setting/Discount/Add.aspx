<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Discount_Add" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Add Discount" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Discount</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Add Discount</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">STORE ACCOUNT</label>
                                <div class="col-lg-7 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlStoreId" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">BY STORE ID</label>
                                <div class="col-lg-3 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtStoreId" CssClass="form-control" placeholder="Store Id ..." autocomplete="off"></asp:TextBox>
                                </div>
                                <div class="col-3">
                                    <asp:Button runat="server" ID="btnCheckStore" Text="Check Data" CssClass="btn btn-primary" OnClick="btnCheckStore_Click" />
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-lg-3 col-form-label required">PRICE GROUP</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlPriceGroupId" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label required">DISCOUNT (%)</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="Number" ID="txtDiscount" CssClass="form-control" placeholder="Discount ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">DESCRITPION</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ......" autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label required">ACTIVE</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                        <asp:ListItem Value="1" Text="YES"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="NO"></asp:ListItem>
                                    </asp:DropDownList>
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

                        <div class="card-footer text-end">
                            <asp:Button runat="server" ID="btnSubmit" Text="Submit" CssClass="btn btn-primary" OnClick="btnSubmit_Click" />
                            <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="btn btn-danger" OnClick="btnCancel_Click" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Information</h3>
                        </div>

                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO Discounts VALUES (NEWID(), @StoreId, @PriceGrouId, @Discount, @Description, @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="ddlStoreId" Name="StoreId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="ddlPriceGroupId" Name="PriceGrouId" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtDiscount" Name="Discount" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>

    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", function () {
            loaderFadeOut();
        })
    </script>
</asp:Content>