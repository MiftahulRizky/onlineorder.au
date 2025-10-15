<%@ Page Title="Add Custom Discount" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_CustomDiscount_Add" MaintainScrollPositionOnPostback="true" Debug="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Custom Discount</h2>
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
                            <h3 class="card-title">Add Custom Discount</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">DESIGN TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlDesign" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesign_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-md-3 col-form-label">BLIND TYPE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBlindId" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">BLIND NO</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlBlindNo" CssClass="form-select">
                                        <asp:ListItem Value="" Text=""></asp:ListItem>
                                        <asp:ListItem Value="Blind 1" Text="BLIND 1"></asp:ListItem>
                                        <asp:ListItem Value="Blind 2" Text="BLIND 2"></asp:ListItem>
                                        <asp:ListItem Value="Blind 3" Text="BLIND 3"></asp:ListItem>
                                        <asp:ListItem Value="Blind 4" Text="BLIND 4"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-5 row">
                                <label class="col-lg-3 col-form-label">CUSTOM DISCOUNT NAME</label>
                                <div class="col-lg-9 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Custom Discount Name ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">FORMULA</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlFieldName" CssClass="form-select"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">&nbsp;</label>
                                <div class="col-lg-9 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtFormula" Height="100px" TextMode="MultiLine" CssClass="form-control" placeholder="Formula ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">DISCOUNT (%)</label>
                                <div class="col-lg-9 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtCharge" TextMode="MultiLine" Height="100px" CssClass="form-control" placeholder="Discount ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>
                            
                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">DATE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtFromDate" TextMode="Date" CssClass="form-control" placeholder="Start Date ..." autocomplete="off"></asp:TextBox>
                                    <small class="form-hint">* From Date</small>
                                </div>

                                <div class="col-lg-1 col-md-12 col-sm-12 text-center">
                                    <label class="col-form-label">?</label>
                                </div>   

                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtToDate" TextMode="Date" CssClass="form-control" placeholder="to Date ..." autocomplete="off"></asp:TextBox>
                                    <small class="form-hint">* To Date</small>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">DESCRITPION</label>
                                <div class="col-lg-9 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">ACTIVE</label>
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
        <asp:Label runat="server" ID="lblDesignId"></asp:Label>
        <asp:Label runat="server" ID="lblBlindId"></asp:Label>

        <asp:SqlDataSource runat="server" ID="sdsPage" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO CustomDiscount VALUES (NEWID(), @DesignId, @BlindId, @BlindNo, @Name, @FieldName, @Formula, @Charge, @FromDate, @ToDate, @Description, @Active)">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblDesignId" Name="DesignId" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblBlindId" Name="BlindId" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlBlindNo" Name="BlindNo" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtName" Name="Name" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlFieldName" Name="FieldName" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtFormula" Name="Formula" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCharge" Name="Charge" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtFromDate" Name="FromDate" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtToDate" Name="ToDate" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtDescription" Name="Description" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlActive" Name="Active" PropertyName="SelectedItem.Value" />
            </InsertParameters>
        </asp:SqlDataSource>
    </div>
    <script type="text/javascript">
        document.addEventListener("DOMContentLoaded", () => {
            loaderFadeOut();
        })
    </script>
</asp:Content>

