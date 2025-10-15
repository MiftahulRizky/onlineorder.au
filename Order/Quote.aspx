<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Quote.aspx.vb" Inherits="Order_Quote" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Quote Details" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Order</div>
                    <h2 class="page-title">Quote Details</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row mb-3">
                <div class="col-lg-6 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:Table runat="server" Font-Size="Larger" CellPadding="5">
                                        <asp:TableRow>
                                            <asp:TableCell Width="130px">Order Number</asp:TableCell>
                                            <asp:TableCell Width="15px">:</asp:TableCell>
                                            <asp:TableCell>
                                                <asp:Label runat="server" ID="lblOrderNo"></asp:Label>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                        <asp:TableRow>
                                            <asp:TableCell>Reference</asp:TableCell>
                                            <asp:TableCell>:</asp:TableCell>
                                            <asp:TableCell>
                                                <asp:Label runat="server" ID="lblOrderCust"></asp:Label>
                                            </asp:TableCell>
                                        </asp:TableRow>
                                    </asp:Table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Quote Data</h3>
                        </div>
                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">EMAIL</label>
                                <div class="col-lg-6 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtEmail" TextMode="Email" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">PHONE</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPhone" CssClass="form-control" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">ADDRESS</label>
                                <div class="col-lg-8 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtAddress" TextMode="MultiLine" Height="100px" CssClass="form-control" placeholder="Address ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">SUBURB</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtSuburb" CssClass="form-control" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">STATES</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtStates" CssClass="form-control" placeholder="States ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-6 row">
                                <label class="col-lg-3 col-form-label">POST CODE</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtPostCode" CssClass="form-control" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-1 row">
                                <p style="color:red;"><b><u>PRICING</u></b></p>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">G.S.T</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:DropDownList runat="server" ID="ddlGST" CssClass="form-select">
                                        <asp:ListItem Value="Yes" Text="YES"></asp:ListItem>
                                        <asp:ListItem Value="No" Text="NO"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">DISCOUNT</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtDiscount" TextMode="Number" CssClass="form-control" placeholder="Disc ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">CHECK MEASURE</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtCM" TextMode="Number" CssClass="form-control" placeholder="Check M ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="mb-3 row">
                                <label class="col-lg-3 col-form-label">INSTALLATION</label>
                                <div class="col-lg-2 col-md-12 col-sm-12">
                                    <asp:TextBox runat="server" ID="txtInstall" TextMode="Number" CssClass="form-control" placeholder="Installation ..." autocomplete="off"></asp:TextBox>
                                </div>
                            </div>

                            <div class="row" runat="server" id="divError">
                                <div class="col-lg-12">
                                    <div class="row">
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
                            <h3 class="card-title">Notes</h3>
                        </div>

                        <div class="card-body">
                            <div class="mb-3 row">
                                <p runat="server" id="pNotes"></p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>

        <asp:SqlDataSource ID="sdsPage" runat="server" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Address=@Address, Suburb=@Suburb, States=@States, PostCode=@PostCode, Phone=@Phone, Email=@Email, QuoteGST=@QuoteGST, QuoteDisc=@QuoteDisc, QuoteInstall=@QuoteInstall, QuoteMeasure=@QuoteMeasure, Active=1 WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="ddlGST" Name="QuoteGST" PropertyName="SelectedItem.Value" />
                <asp:ControlParameter ControlID="txtDiscount" Name="QuoteDisc" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtInstall" Name="QuoteInstall" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtCM" Name="QuoteMeasure" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtAddress" Name="Address" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtSuburb" Name="Suburb" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtStates" Name="States" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtPostCode" Name="PostCode" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtPhone" Name="Phone" PropertyName="Text" />
                <asp:ControlParameter ControlID="txtEmail" Name="Email" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>