<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Order_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Detail Order" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col-lg-4 col-sm-12 col-md-12">
                    <div class="page-pretitle">Order</div>
                    <h2 class="page-title">Detail Order</h2>
                </div>

                <div class="col-lg-8 col-sm-12 col-md-12 text-end">
                    <asp:Button runat="server" ID="btnPreview" CssClass="btn" Text="Preview" OnClick="btnPreview_Click" />
                    <asp:Button runat="server" ID="btnEditHeader" CssClass="btn btn-primary" Text="Edit" OnClick="btnEditHeader_Click" />
                    <a href="#" runat="server" id="aDeleteOrder" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDeleteOrder">Delete</a>
                    <a href="#" runat="server" id="aSubmitOrder" class="btn btn-success" data-bs-toggle="modal" data-bs-target="#modalSubmitOrder">Submit</a>
                    <a runat="server" id="aOtorisasi" class="btn btn-secondary position-relative" data-bs-toggle="offcanvas" href="#canvasOtorisasi" role="button" aria-controls="canvasAdd">
                        Authorization <span runat="server" id="spanOtorisasi" class="badge bg-red text-blue-fg badge-notification badge-pill">0</span></a>
                    <button class="btn dropdown-toggle" data-bs-toggle="dropdown" runat="server" id="btnApproval">Approval Order</button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <a href="#" runat="server" id="aAuthorize" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalAuthorizeOrder">Authorize Order</a>
                        <a href="#" runat="server" id="aDecline" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeclineOrder">Decline Order</a>
                    </div>
                    <a href="#" runat="server" id="aGenerateJob" class="btn btn-info" data-bs-toggle="modal" data-bs-target="#modalGenerateJob">Generate Job</a>
                    <a href="#" runat="server" id="aUnsubmitOrder" class="btn btn-info" data-bs-toggle="modal" data-bs-target="#modalUnsubmitOrder">Unsubmit</a>
                    <a href="#" runat="server" id="aCancelOrder" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalCancelOrder">Cancel</a>
                    <a href="#" runat="server" id="aCompleteOrder" class="btn btn-orange" data-bs-toggle="modal" data-bs-target="#modalCompleteOrder">Complete</a>
                    <a href="#" runat="server" id="aSlip" class="btn btn-orange" data-bs-toggle="modal" data-bs-target="#modalPaperlessSlip">Peperless Slip</a>
                    <a href="#" runat="server" id="aExact" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalExactSlip">Exact Slip</a>
                    <button class="btn dropdown-toggle" data-bs-toggle="dropdown" runat="server" id="btnQuoteAction">Quote</button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <a href="#" runat="server" id="aQuoteDetail" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalQuoteDetail">Quote Details</a>
                        <asp:Button runat="server" ID="btnQuoteDownload" CssClass="dropdown-item" Text="Download Quote" OnClick="btnQuoteDownload_Click" OnClientClick="return showWaitingQuote();" />
                    </div>
                    <button class="btn btn-purple dropdown-toggle" data-bs-toggle="dropdown" runat="server" id="btnMoreAction">More</button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <a href="#" runat="server" id="aStatusAdditional" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalStatusAdditional">Additional Status</a>
                        <a href="#" runat="server" id="aInternalNote" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalInternalNote">Internal Note</a>
                        <div class="dropdown-divider" runat="server" id="divDividerQuote"></div>
                        <asp:Button runat="server" ID="btnQuotePrint" CssClass="dropdown-item" Text="Print Quote" OnClick="btnQuotePrint_Click" />
                        <a href="#" runat="server" id="aQuoteEmail" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalQuoteEmail">Email Quote</a>
                        <div class="dropdown-divider" runat="server" id="divDividerDeposit"></div>
                        <a href="#" runat="server" id="aDeposit" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeposit">Email Deposit Request</a>
                        <a href="#" runat="server" id="aHuake" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalHuake">Send Email Huake</a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="page-body">
        <div class="container-xl">
            <div class="row">
                <div class="col-12">
                    <div runat="server" id="divSuccess" class="alert alert-important alert-success alert-dismissible" role="alert">
                        <div class="d-flex">
                            <div>
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M5 12l5 5l10 -10" /></svg>
                            </div>
                            <div>
                                <span runat="server" id="msgSuccess"></span>
                            </div>
                        </div>
                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                    </div>

                    <div runat="server" id="divError" class="alert alert-important alert-danger alert-dismissible" role="alert">
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
                <div class="col-lg-7">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4" runat="server" id="divRetailerName">
                                <div class="col">
                                    <span style="font-size:larger;">Retailer Name :</span>
                                    <br />
                                    <span runat="server" id="spanCustomerName" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                            <div class="row mb-4">
                                <div class="col-4">
                                    <span style="font-size:larger;">Order # :</span>
                                    <br />
                                    <span runat="server" id="spanOrderId" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                <div class="col-4">
                                    <span style="font-size:larger;">Customer Order Number :</span>
                                    <br />
                                    <span runat="server" id="spanOrderNumber" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                <div class="col-4">
                                    <span style="font-size:larger;">Customer Order Name :</span>
                                    <br />
                                    <span runat="server" id="spanOrderName" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>

                            <div class="row mb-4">
                                <div class="col-4">
                                    <span style="font-size:larger;">Customer Note :</span>
                                    <br />
                                    <span runat="server" id="spanOrderNote" style="font-size:small;font-weight:bold;"></span>
                                </div>

                                <div class="col-4">
                                    <span style="font-size:larger;">Status :</span>
                                    <br />
                                    <span runat="server" id="spanStatusOrder" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-4">
                                    <span style="font-size:larger;">Additional Status :</span>
                                    <br />
                                    <span runat="server" id="spanStatusAdditional" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-6">
                                    <span style="font-size:larger;">Canceled Reason :</span>
                                    <br />
                                    <span runat="server" id="spanCanceledNote" style="font-size:small;font-weight:bold;"></span>
                                </div>

                                <div class="col-6" runat="server" id="divInternalNote">
                                    <span style="font-size:larger;">Internal Note :</span>
                                    <br />
                                    <span runat="server" id="spanInternalNote" style="font-size:small;font-weight:bold;"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-lg-5">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-6">
                                    <span style="font-size:larger;">Created By :</span>
                                    <br />
                                    <span runat="server" id="spanCreatedBy" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                <div class="col-6">
                                    <span style="font-size:larger;">Created Date :</span>
                                    <br />
                                    <span runat="server" id="spanCreatedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>

                            <div class="row mb-4">
                                <div class="col-4">
                                    <span style="font-size:larger;">Submitted Date :</span>
                                    <br />
                                    <span runat="server" id="spanSubmittedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                <div class="col-4">
                                    <span style="font-size:larger;">Completed Date :</span>
                                    <br />
                                    <span runat="server" id="spanCompletedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                <div class="col-4">
                                    <span style="font-size:larger;">Canceled Date :</span>
                                    <br />
                                    <span runat="server" id="spanCanceledDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-6">
                                    <span style="font-size:larger;">Job Number :</span>
                                    <br />
                                    <span runat="server" id="spanJobId" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                <div class="col-6">
                                    <span style="font-size:larger;">Job Date :</span>
                                    <br />
                                    <span runat="server" id="spanJobDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-lg-6 col-sm-12 col-md-12">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-4">
                                    <span style="font-size:larger;">Shipment # :</span>
                                    <br />
                                    <span runat="server" id="spanShipmentNo" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-4">
                                    <span style="font-size:larger;">ETA to Port :</span>
                                    <br />
                                    <span runat="server" id="spanShipmentPort" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                                
                                <div class="col-4">
                                    <span style="font-size:larger;">ETA to Customer :</span>
                                    <br />
                                    <span runat="server" id="spanShipmentCustomer" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6 col-sm-12 col-md-12">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-4">
                                    <span style="font-size:larger;">Total excl. GST :</span>
                                    <br />
                                    <span runat="server" id="spanTotal" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-4">
                                    <span style="font-size:larger;">GST :</span>
                                    <br />
                                    <span runat="server" id="spanGST" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                 <div class="col-4">
                                    <span style="font-size:larger;">TOTAL incl. GST :</span>
                                    <br />
                                    <span runat="server" id="spanFinalTotal" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-12">                    
                    <div class="card">
                        <div class="card-header">
                            <h2 class="card-title">YOUR ITEMS</h2>
                            <div class="card-actions">
                                <a href="#" runat="server" id="aAddItem" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalAddItem">New Item</a>
                                <asp:Button runat="server" ID="btnService" CssClass="btn btn-primary" Text="New Service" OnClick="btnService_Click" />
                            </div>
                        </div>

                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" PagerSettings-Position="Top" EmptyDataText="ORDER ITEM NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="100" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                <RowStyle />
                                <Columns>
                                    <asp:BoundField DataField="Id" HeaderText="ID" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                    <asp:BoundField DataField="Id" HeaderText="ID" />
                                    <asp:BoundField DataField="Number" HeaderText="#" />
                                    <asp:BoundField DataField="Qty" HeaderText="Qty" />
                                    <asp:TemplateField HeaderText="Item Description">
                                        <ItemTemplate>
                                            <%# BindProductDescription(Eval("Id").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cost">
                                        <ItemTemplate>
                                            <%# ShowingPrice(Eval("FinalCost")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mark Up">
                                        <ItemTemplate>
                                            <%# BindMarkUp(Eval("MarkUp")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Production" HeaderText="Factory" />
                                    <asp:TemplateField ItemStyle-Width="150px">
                                        <ItemTemplate>
                                            <button class="btn btn-sm btn-pill btn-orange" data-bs-toggle="dropdown">Actions</button>
                                            <div class="dropdown-menu dropdown-menu-end">
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkDetail" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>' Visible='<%# VisibleDetail(Eval("ProductId").ToString())%>'></asp:LinkButton>

                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkCopy" Text="Copy" CommandName="Copy" CommandArgument='<%# Eval("Id") %>' Visible='<%# VisibleCopy(Eval("ProductId").ToString())%>'></asp:LinkButton>

                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteItem" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), Eval("Number").ToString(), "Detail") %>' visible='<%# VisibleDelete(Eval("ProductId").ToString()) %>'>Delete</a>

                                                <%--START PRODUCTION--%>
                                                <div class="dropdown-divider" runat="server" visible='<%# VisibleProduction(Eval("ProductId").ToString(), Eval("Production").ToString()) %>'></div>
                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalProduction" onclick='<%# String.Format("return showProduction(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Production").ToString()) %>' visible='<%# VisibleProduction(Eval("ProductId").ToString(), Eval("Production").ToString()) %>'>Change Production</a>
                                                <%--END PRODUCTION--%>

                                                <%--START PRICING--%>
                                                <div class="dropdown-divider"></div>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkPriceInfo" Text="Pricing Info" CommandName="InfoPrice" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkEditPricing" Text="Edit Pricing" Visible='<%# VisibleEditPricing()%>' CommandName="EditPrice" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                <%--END PRICING--%>

                                                <div class="dropdown-divider"></div>
                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkLog" Text="Logs" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                            </div>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="Blue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>
                        <div class="card-footer"></div>
                    </div>
                </div>
            </div>

            <div class="row mt-3" runat="server" id="divErrorB">
                <div class="col-12">
                    <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                        <div class="d-flex">
                            <div>
                                <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                            </div>
                            <div>
                                <span runat="server" id="msgErrorB"></span>
                            </div>
                        </div>
                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <iframe id="downloadFrame" style="display:none;"></iframe>
    
    <div class="modal modal-blur fade" id="modalSubmitOrder" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-success"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-success icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M17 10l-2 -6" /><path d="M7 10l2 -6" /><path d="M11 20h-3.756a3 3 0 0 1 -2.965 -2.544l-1.255 -7.152a2 2 0 0 1 1.977 -2.304h13.999a2 2 0 0 1 1.977 2.304l-.479 2.729" /><path d="M10 14a2 2 0 1 0 4 0a2 2 0 0 0 -4 0" /><path d="M15 19l2 2l4 -4" />
                    </svg>
                    <h3 runat="server" id="titleSubmit">Submit Order</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" id="cancelSubmit" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitOrder" CssClass="btn w-100 btn-success" Text="Confirm" OnClick="btnSubmitOrder_Click" OnClientClick="return showWaiting();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal modal-blur fade" id="modalDeleteOrder" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Order</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitDelete" CssClass="btn btn-danger w-100" Text="Confirm" OnClick="btnSubmitDelete_Click" OnClientClick="return showWaiting();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalGenerateJob" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-info"></div>
                <div class="modal-body text-center py-4">
                    <h3>Generate Job</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                    <div class="text-secondary mt-3" runat="server" id="divMinimumSurcharge">
                        <b>This is a minimum surcharge customer.</b><br />
                        Please make sure you have added the minimum additional charge to this order.
                    </div>
                    <div class="text-secondary mt-3" runat="server" id="divGenerateJob">
                        <b>This is a cash sale customer.</b><br />
                        Please make sure a deposit request has been sent for this order.
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitGenerateJob" CssClass="btn w-100 btn-info" Text="Confirm" OnClick="btnSubmitGenerateJob_Click" OnClientClick="return showWaiting();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalCancelOrder" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Cancel Order</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row mb-4">
                        <div class="col-12">
                            <label class="form-label required">Reason Category</label>
                            <asp:DropDownList runat="server" ID="ddlReasonCategory" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Staff Error" Text="STAFF ERROR"></asp:ListItem>
                                <asp:ListItem Value="Customer Error" Text="CUSTOMER ERROR"></asp:ListItem>
                                <asp:ListItem Value="Requested by Customer" Text="REQUESTED BY CUSTOMER"></asp:ListItem>
                                <asp:ListItem Value="Lost to Competitor" Text="LOST TO COMPETITOR"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">Reason Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtCancelReasonDesc" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorCancelOrder">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorCancelOrder"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitCancel" CssClass="btn btn-danger" Text="Submit" OnClick="btnSubmitCancel_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalUnsubmitOrder" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-info"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-info icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Unsubmit Order</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitUnsubmit" CssClass="btn w-100 btn-info" Text="Confirm" OnClick="btnSubmitUnsubmit_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalCompleteOrder" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-orange"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-orange icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 9h8" /><path d="M8 13h6" /><path d="M10.99 19.206l-2.99 1.794v-3h-2a3 3 0 0 1 -3 -3v-8a3 3 0 0 1 3 -3h12a3 3 0 0 1 3 3v6" /><path d="M15 19l2 2l4 -4" />
                    </svg>
                    <h3>Complete Order</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitComplete" CssClass="btn w-100 btn-orange" Text="Confirm" OnClick="btnSubmitComplete_Click" OnClientClick="return showWaiting();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalQuoteDetail" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Quote Details</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-6">
                            <label class="form-label">EMAIL</label>
                            <asp:TextBox runat="server" ID="txtQuoteEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6">
                            <label class="form-label">PHONE</label>
                            <asp:TextBox runat="server" ID="txtQuotePhone" CssClass="form-control" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-6">
                            <label class="form-label">ADDRESS</label>
                            <asp:TextBox runat="server" ID="txtQuoteAddress" CssClass="form-control" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6">
                            <label class="form-label">SUBURB</label>
                            <asp:TextBox runat="server" ID="txtQuoteSuburb" CssClass="form-control" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-5">
                        <div class="col-6">
                            <label class="form-label">STATES</label>
                            <asp:DropDownList runat="server" ID="ddlQuoteStates" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="ACT" Text="ACT"></asp:ListItem>
                                <asp:ListItem Value="NSW" Text="NSW"></asp:ListItem>
                                <asp:ListItem Value="NT" Text="NT"></asp:ListItem>
                                <asp:ListItem Value="QLD" Text="QLD"></asp:ListItem>
                                <asp:ListItem Value="SA" Text="SA"></asp:ListItem>
                                <asp:ListItem Value="TAS" Text="TAS"></asp:ListItem>
                                <asp:ListItem Value="VIC" Text="VIC"></asp:ListItem>
                                <asp:ListItem Value="WA" Text="WA"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-6">
                            <label class="form-label">POST CODE</label>
                            <asp:TextBox runat="server" ID="txtQuotePostCode" CssClass="form-control" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-6">
                            <label class="form-label">DISCOUNT</label>
                            <div class="input-group">
                                <span class="input-group-text">$</span>
                                <asp:TextBox runat="server" ID="txtQuoteDiscount" CssClass="form-control" placeholder="Discount ..." autocomplete="off"></asp:TextBox>
                            </div>
                            <small class="form-hint">* Please use decimal separator with dot (.)</small>
                        </div>
                        <div class="col-6">
                            <label class="form-label">INSTALLATION</label>
                            <div class="input-group">
                                <span class="input-group-text">$</span>
                                <asp:TextBox runat="server" ID="txtQuoteInstallation" CssClass="form-control" placeholder="Installation ..." autocomplete="off"></asp:TextBox>
                            </div>
                            <small class="form-hint">* Please use decimal separator with dot (.)</small>
                        </div>
                    </div>

                    <div class="row mb-4">
                        <div class="col-6">
                            <label class="form-label">CHECK MEASURE</label>
                            <div class="input-group">
                                <span class="input-group-text">$</span>
                                <asp:TextBox runat="server" ID="txtQuoteCheckMeasure" CssClass="form-control" placeholder="Check Measure ..." autocomplete="off"></asp:TextBox>
                            </div>
                            <small class="form-hint">* Please use decimal separator with dot (.)</small>
                        </div>
                        <div class="col-6">
                            <label class="form-label">FREIGHT</label>
                            <div class="input-group">
                                <span class="input-group-text">$</span>
                                <asp:TextBox runat="server" ID="txtQuoteFreight" CssClass="form-control" placeholder="Freight ..." autocomplete="off"></asp:TextBox>
                            </div>
                            <small class="form-hint">* Please use decimal separator with dot (.)</small>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorQuoteDetail">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorQuoteDetail"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <div class="w-33">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitQuoteDetail" CssClass="btn btn-primary w-100" Text="Submit" OnClick="btnSubmitQuoteDetail_Click" OnClientClick="return showWaiting();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalHuake" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-orange"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-orange icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 9h8" /><path d="M8 13h6" /><path d="M10.99 19.206l-2.99 1.794v-3h-2a3 3 0 0 1 -3 -3v-8a3 3 0 0 1 3 -3h12a3 3 0 0 1 3 3v6" /><path d="M15 19l2 2l4 -4" />
                    </svg>
                    <h3>Email Job Order</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitHuake" CssClass="btn w-100 btn-orange" OnClick="btnSubmitHuake_Click" Text="Confirm" OnClientClick="return showWaiting();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalExactSlip" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-orange"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-orange icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 9h8" /><path d="M8 13h6" /><path d="M10.99 19.206l-2.99 1.794v-3h-2a3 3 0 0 1 -3 -3v-8a3 3 0 0 1 3 -3h12a3 3 0 0 1 3 3v6" /><path d="M15 19l2 2l4 -4" />
                    </svg>
                    <h3>Create XML Exact</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                        <br /><br />
                        There is no success message if successful. Please check Exact directly.
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitExact" OnClick="btnSubmitExact_Click" CssClass="btn w-100 btn-orange" Text="Confirm" OnClientClick="return showWaiting();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalPaperlessSlip" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-orange"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon mb-2 text-orange icon-lg">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" /><path d="M8 9h8" /><path d="M8 13h6" /><path d="M10.99 19.206l-2.99 1.794v-3h-2a3 3 0 0 1 -3 -3v-8a3 3 0 0 1 3 -3h12a3 3 0 0 1 3 3v6" /><path d="M15 19l2 2l4 -4" />
                    </svg>
                    <h3>Paperless Slip</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                        <br /><br />
                        There is no success message if successful. Please check Micronet directly.
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitSlip" CssClass="btn w-100 btn-orange" Text="Confirm" OnClick="btnSubmitSlip_Click" OnClientClick="return handleSubmitClick();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="offcanvas offcanvas-end" tabindex="-1" id="canvasOtorisasi" aria-labelledby="canvasAddLabel">
        <div class="offcanvas-header">
            <h2 class="offcanvas-title" id="canvasAddLabel">Authorisation</h2>
            <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
        </div>
        <div class="offcanvas-body">
            <asp:ListView runat="server" ID="lvOtorisasi" OnItemCommand="lvOtorisasi_ItemCommand">
                <ItemTemplate>
                    <div class="row row-cards mb-3">
                        <div class="col-lg-12">
                            <div class="card">
                                <div class="card-header">
                                    <h5 class="card-title">Out Of Spec</h5>
                                    <div class="card-actions">
                                        <asp:Button runat="server" Text="Authorize" CssClass="btn btn-success" Visible='<%# VisibleAuthorizeDeclineListView(Eval("Status").ToString()) %>' CommandName="Authorize" CommandArgument='<%# Eval("Id").ToString() %>' />
                                        <asp:Button runat="server" Text="Decline" CssClass="btn btn-danger" Visible='<%# VisibleAuthorizeDeclineListView(Eval("Status").ToString()) %>' CommandName="Decline" CommandArgument='<%# Eval("Id").ToString() %>' />
                                        <asp:Button runat="server" Text="Reset" CssClass="btn" Visible='<%# VisibleResetListView(Eval("Status").ToString()) %>' CommandName="Reset" CommandArgument='<%# Eval("Id").ToString() %>' />
                                    </div>
                                </div>
                                <div class="card-body">
                                    <h5 class="card-title">Order# <%# Eval("JobOtorisasi").ToString() %></h5>
                                    <span>- <%# Eval("Description").ToString() %></span>

                                    <span runat="server" class='<%# BindStyleAuthorization(Eval("Status").ToString()) %>'><%# BindTextAuthorization(Eval("Status").ToString(), Eval("LoginId").ToString()) %></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalAuthorizeOrder" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-info"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-info icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Authorize Order</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitAuthorizeOrder" CssClass="btn btn-info w-100" Text="Confirm" OnClick="btnSubmitAuthorizeOrder_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeclineOrder" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-info"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-info icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Decline Order</h3>
                    <div class="text-secondary">
                        Are you sure you would like to do this?
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitDeclineOrder" CssClass="btn btn-info w-100" Text="Confirm" OnClick="btnSubmitDeclineOrder_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalStatusAdditional" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Additional Status</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">Additional Status</label>
                            <asp:DropDownList runat="server" ID="ddlStatusAdditional" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorStatusAdditional">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorStatusAdditional"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitStatusAdditional" CssClass="btn btn-purple w-100" Text="Submit" OnClick="btnSubmitStatusAdditional_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalInternalNote" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Internal Note</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label">Note</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtInternalNote" Height="100px" CssClass="form-control" placeholder="Note ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorInternalNote">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorInternalNote"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitIntenalNote" CssClass="btn btn-purple" Text="Submit" OnClick="btnSubmitIntenalNote_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalQuoteEmail" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Email Quote</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">FROM</label>
                            <asp:TextBox runat="server" ID="txtEmailQuoteFrom" CssClass="form-control" placeholder="FROM ..." autocomplete="off" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">TO</label>
                            <asp:TextBox runat="server" ID="txtEmailQuoteTo" CssClass="form-control" placeholder="TO ..." autocomplete="off"></asp:TextBox>
                            <small class="form-hint">This is taken from customers primary contact email address if available.</small>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">CC</label>
                            <asp:TextBox runat="server" ID="txtEmailQuoteCC" CssClass="form-control" placeholder="CC ..." autocomplete="off"></asp:TextBox>
                            <small class="form-hint">Will be cc'ed to <b>Customer Service</b> and <b>Accounts</b> by default.</small>
                        </div>
                    </div>

                     <div class="row" runat="server" id="divErrorQuoteEmail">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorQuoteEmail"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitQuoteEmail" CssClass="btn btn-purple w-100" Text="Submit" OnClick="btnSubmitQuoteEmail_Click" OnClientClick="return showWaiting();" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>    
    
    <div class="modal modal-blur fade" id="modalDeposit" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Email Deposit Request</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">FROM</label>
                            <asp:TextBox runat="server" ID="txtDepositFrom" CssClass="form-control" placeholder="FROM ..." autocomplete="off" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">TO</label>
                            <asp:TextBox runat="server" ID="txtDepositTo" CssClass="form-control" placeholder="TO ..." autocomplete="off"></asp:TextBox>
                            <small class="form-hint">This is taken from customers primary contact email address if available.</small>
                        </div>
                    </div>

                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">CC</label>
                            <asp:TextBox runat="server" ID="txtDepositCc" CssClass="form-control" placeholder="CC ..." autocomplete="off"></asp:TextBox>
                            <small class="form-hint">Will be cc'ed to <b>Customer Service</b> and <b>Accounts</b> by default.</small>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorDeposit">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorDeposit"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitDeposit" CssClass="btn btn-purple" Text="Submit" OnClick="btnSubmitDeposit_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalAddItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Item</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label required">SELECT PRODUCT</label>
                            <asp:DropDownList runat="server" ID="ddlDesign" CssClass="form-select"></asp:DropDownList>
                            <small class="form-hint" style="color:red;">* Please select a product then click the submit button</small>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitAddItem" CssClass="btn btn-primary w-100" Text="Submit" OnClick="btnSubmitAddItem_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal modal-blur fade" id="modalPricing" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Price Details</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="card-body border-bottom py-3" runat="server" id="divErrorPricing">
                        <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                            <div class="d-flex">
                                <div>
                                    <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                </div>
                                <div>
                                    <span runat="server" id="msgErrorPricing"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvListDetailPrice" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" EmptyDataText="ORDER ITEM NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center">
                            <RowStyle />
                            <Columns>
                                <asp:BoundField DataField="Id" HeaderText="ID" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                                <asp:BoundField DataField="Type" HeaderText="Type" />
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:BoundField DataField="FinalPrice" HeaderText="Price" />
                            </Columns>
                            <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                    </div>
                    <div class="card-footer"></div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeleteItem" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-danger"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" class="icon mb-2 text-danger icon-lg" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M10.24 3.957l-8.422 14.06a1.989 1.989 0 0 0 1.7 2.983h16.845a1.989 1.989 0 0 0 1.7 -2.983l-8.423 -14.06a1.989 1.989 0 0 0 -3.4 0z" /><path d="M12 9v4" /><path d="M12 17h.01" /></svg>
                    <h3>Delete Item Order</h3>
                    <div class="text-secondary">
                        <asp:TextBox runat="server" ID="txtDeleteId" style="display:none;"></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtDeleteNumber" style="display:none;"></asp:TextBox>
                        <span id="spanDelete"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitDeleteItem" CssClass="btn btn-danger w-100" Text="Confirm" OnClick="btnSubmitDeleteItem_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalEditPricing" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Pricing</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtEditPricingId" style="display:none;"></asp:TextBox>
                    <div class="row mb-3">
                        <div class="col-12">
                            <label class="form-label">Base Price</label>
                            <asp:Label runat="server" ID="lblBasePrice" CssClass="form-label"></asp:Label>
                        </div>
                    </div>

                    <div class="row mb-5">
                        <div class="col-12">
                            <label class="form-label">Override Price</label>
                            <div class="input-group">
                                <span class="input-group-text">$</span>
                                <asp:TextBox runat="server" ID="txtOverridePrice" CssClass="form-control" placeholder="..." autocomplete="off"></asp:TextBox>                                
                            </div>
                            <small class="form-hint">* Please use decimal separator with dot (.)</small>
                        </div>
                    </div>

                    <div class="row mb-5">
                        <div class="col-6">
                            <label class="form-label">Discount Type</label>
                            <asp:DropDownList runat="server" ID="ddlDiscountType" CssClass="form-select" ClientIDMode="Static" Width="50%">
                                <asp:ListItem Value="$" Text="$"></asp:ListItem>
                                <asp:ListItem Value="%" Text="%"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-5">
                        <div class="col-12">
                            <label class="form-label">Additional Discount</label>
                            <div class="input-group">
                                <span class="dollar input-group-text">$</span>
                                <asp:TextBox runat="server" ID="txtDiscount" CssClass="form-control" placeholder="Discount ..." autocomplete="off"></asp:TextBox>
                                <span class="percent input-group-text">%</span>
                            </div>
                            <small class="form-hint">* Please use decimal separator with dot (.)</small>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorEditPricing">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorEditPricing"></span>
                                    </div>
                                </div>
                                <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitEditPricing" CssClass="btn btn-primary w-100" Text="Submit" OnClick="btnSubmitEditPricing_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalProduction" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Change Production</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtProductionId" style="display:none;"></asp:TextBox>
                    <div class="row mb-5">
                        <div class="col-12">
                            <label class="form-label required">Choose Factory</label>
                            <asp:DropDownList runat="server" ID="ddlProduction" CssClass="form-select" ClientIDMode="Static">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="JKT" Text="JKT"></asp:ListItem>
                                <asp:ListItem Value="Orion" Text="Orion"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmitOrion" CssClass="btn btn-orange w-100" Text="Submit" OnClick="btnSubmitOrion_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" role="dialog" aria-hidden="true">
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
                        <asp:GridView runat="server" ID="gvListLogs" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" EmptyDataText="DATA LOG NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeader="false" GridLines="None" BorderStyle="None">
                            <RowStyle />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%# BindTextLog(Eval("Id").ToString()) %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalPreview" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-full-width modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Preview Order</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
            
                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorPreview">
                        <div class="col-12">
                            <div class="alert alert-important alert-danger alert-dismissible" role="alert">
                                <div class="d-flex">
                                    <div>
                                        <svg xmlns="http://www.w3.org/2000/svg" class="icon alert-icon" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="currentColor" fill="none" stroke-linecap="round" stroke-linejoin="round"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M3 12a9 9 0 1 0 18 0a9 9 0 0 0 -18 0" /><path d="M12 8v4" /><path d="M12 16h.01" /></svg>
                                    </div>
                                    <div>
                                        <span runat="server" id="msgErrorPreview"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <iframe id="framePreview" runat="server" width="100%" height="600px" style="border: none;"></iframe>
                </div>

                <div class="modal-footer"></div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalWaiting" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-status bg-secondary"></div>
                <div class="modal-body text-center py-4">
                    <svg  xmlns="http://www.w3.org/2000/svg"  width="24"  height="24"  viewBox="0 0 24 24"  fill="none"  stroke="currentColor"  stroke-width="2"  stroke-linecap="round"  stroke-linejoin="round"  class="icon mb-2 text-secondary icon-lg"><path stroke="none" d="M0 0h24v24H0z" fill="none"/><path d="M6 20v-2a6 6 0 1 1 12 0v2a1 1 0 0 1 -1 1h-10a1 1 0 0 1 -1 -1z" /><path d="M6 4v2a6 6 0 1 0 12 0v-2a1 1 0 0 0 -1 -1h-10a1 1 0 0 0 -1 1z" /></svg>
                    <h3>Please wait ......</h3>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">

        document.addEventListener("DOMContentLoaded", function () {
            loaderFadeOut();
        })
        
        function showWaiting() {
            $("#modalWaiting").modal("show");
        }

        function showWaitingQuote() {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
            }, 2000);
        }

        ["modalSubmitOrder", "modalDeleteOrder", "modalGenerateJob", "modalCancelOrder", "modalCompleteOrder", "modalQuoteDetail", "modalAddItem", "modalStatusAdditional", "modalDeposit", "modalQuoteEmail", "modalHuake", "modalWaiting"].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        $(document).on("show.bs.modal", "#modalEditPricing", function () {
            visibleDiscountType();            
        });

        function showLog() {
            $("#modalLog").modal("show");
        }

        function showPreview() {
            $("#modalPreview").modal("show");
        }

        $("#ddlDiscountType").on("change", function (e) {
            e.preventDefault();
            visibleDiscountType();
            document.getElementById("<%=txtDiscount.ClientID %>").value = "";
        });

        function visibleDiscountType() {
            var type = document.getElementById("<%=ddlDiscountType.ClientID %>").value;
            if (type === '$') {
                $(".dollar").show();
                $(".percent").hide();
            } else if (type === '%') {
                $(".dollar").hide();
                $(".percent").show();
            } else {
                $(".dollar").hide();
                $(".percent").hide();
            }
        }

        function showDelete(id, number) {
            let description = "Are you sure you want to delete item order? <br />";
            document.getElementById("<%=txtDeleteId.ClientID %>").value = id;
            document.getElementById("<%=txtDeleteNumber.ClientID %>").value = number;
            document.getElementById("spanDelete").innerHTML = description;
        }

        function showProduction(id, production) {
            document.getElementById("<%=txtProductionId.ClientID %>").value = id;
            document.getElementById("<%=ddlProduction.ClientID %>").value = production;
        }

        function showCanvasOtorisasi() {
            const myOffcanvas = new bootstrap.Offcanvas(document.getElementById("canvasOtorisasi"));
            myOffcanvas.show();
        }

        function showStatusAdditional() {
            $("#modalStatusAdditional").modal("show");
        }

        function showDetailPrice() {
            $("#modalPricing").modal("show");
        }

        function showEditPricing() {
            $('#modalEditPricing').modal("show");
        }

        function showQuoteDetail() {
            $("#modalQuoteDetail").modal("show");
        }

        function showQuoteEmail() {
            $("#modalQuoteEmail").modal("show");
        }

        function showDeposit() {
            $("#modalDeposit").modal("show");
        }

        function showCancelOrder() {
            $("#modalCancelOrder").modal("show");
        }

        function showInternalNote() {
            $("#modalInternalNote").modal("show");
        }
    </script>

<div runat="server" visible="false">
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblMnetId"></asp:Label>
        <asp:Label runat="server" ID="lblCreatedBy"></asp:Label>
        <asp:Label runat="server" ID="lblJobId"></asp:Label>
        <asp:Label runat="server" ID="lblShipmentId"></asp:Label>
        <asp:Label runat="server" ID="lblApproved"></asp:Label>
        <asp:Label runat="server" ID="lblStatusAdditional"></asp:Label>
        <asp:Label runat="server" ID="lblOrderType"></asp:Label>
        
        <asp:Label runat="server" ID="lblItemId"></asp:Label>
        <asp:Label runat="server" ID="lblSupplierAction"></asp:Label>
        <asp:Label runat="server" ID="lblCashSale"></asp:Label>
        <asp:Label runat="server" ID="lblDeposit"></asp:Label>
    </div>
</asp:Content>
