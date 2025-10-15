<%@ Page Language="VB" AutoEventWireup="true" CodeFile="Detail.aspx.vb" Inherits="Order_Detail" MaintainScrollPositionOnPostback="true" MasterPageFile="~/Site.master" Debug="true" Title="Detail Order" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col-lg-2 col-sm-12 col-md-12">
                    <div class="page-pretitle">Order</div>
                    <h2 class="page-title">Order Details</h2>
                </div>
                <div class="col-lg-10 col-sm-12 col-md-12 text-end">
                    <!-- button finish -->
                    <asp:LinkButton runat="server" ID="btnFinish" CssClass="btn  btn-cyan" Text="Finish" OnClick="btnFinish_Click" >
                        <i class="bi bi-check-lg me-2"></i>
                        Finish
                    </asp:LinkButton>

                    <!-- button preview -->
                    <button class="btn  btn-secondary dropdown-toggle" data-bs-toggle="dropdown">
                        <i class="bi bi-file-earmark-text me-2"></i>
                        Preview
                    </button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <asp:LinkButton runat="server" ID="btnPreviewPrint" CssClass="dropdown-item" Text="Print" OnClick="btnPreviewPrint_Click" >
                            <i class="bi bi-printer me-2 opacity-50"></i> Print
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" ID="btnPreviewPDF" CssClass="dropdown-item" Text="Download PDF" OnClick="btnPreviewPDF_Click" >
                            <i class="bi bi-file-earmark-pdf me-2 opacity-50"></i> Download PDF
                        </asp:LinkButton>
                    </div>
                    

                    <!-- button creat job sheet -->
                    <button runat="server" ID="btnJobSheet" class="btn btn-outline-teal dropdown-toggle " type="button" data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="bi bi-file-text me-2"></i> Job Sheet
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li>
                            <asp:LinkButton runat="server" ID="btnConvertToJob" CssClass="dropdown-item" Text="Print" OnClick="btnConvertToJob_Click" OnClientClick="return confirmConvertToJob();">
                                <i class="bi bi-file-earmark-zip me-2 opacity-50"></i> Convert To Job
                            </asp:LinkButton>
                        </li>
                        <li>
                            <asp:LinkButton runat="server" ID="btnReprintJobSheet" CssClass="dropdown-item" Text="Print" OnClick="btnReprintJobSheet_Click">
                                <i class="bi bi-printer me-2 opacity-50"></i> Reprint Job Sheet
                            </asp:LinkButton>
                        </li>
                        <li>
                            <asp:LinkButton runat="server" ID="btnDownloadJobSheet" CssClass="dropdown-item" Text="Print" OnClick="btnDownloadJobSheet_Click">
                                <i class="bi bi-cloud-arrow-down me-2 opacity-50"></i> Download Job Sheet
                            </asp:LinkButton>
                        </li>
                    </ul>

                    <!-- button submit -->
                    <a href="#" runat="server" id="aSubmit" class="btn  btn-success" data-bs-toggle="modal" data-bs-target="#modalSubmit">
                        <i class="fa-regular  fa-paper-plane me-2" ></i> Submit
                    </a>

                    <!-- button edit -->
                    <asp:LinkButton runat="server" ID="btnEditHeader" CssClass="btn  btn-primary" Text="Edit" OnClick="btnEditHeader_Click" >
                        <i class="fa-regular  fa-pen-to-square me-2" ></i> Edit
                    </asp:LinkButton>

                    <!-- button delete -->
                    <a href="#" runat="server" id="aDeleteHeader" class="btn  btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick="showDelete('', 'Header');">
                        <i class="fa-regular  fa-trash-can me-2" ></i> Delete
                    </a>

                    <!-- button quote -->
                    <button class="btn  btn-dark dropdown-toggle" data-bs-toggle="dropdown" runat="server" id="btnQuote">
                        <i class="bi bi-chat-right-quote me-2" ></i> Quote
                    </button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <asp:LinkButton runat="server" ID="btnQuoteDetail" CssClass="dropdown-item" Text="Detail Quote" OnClick="btnQuoteDetail_Click" >
                            <i class="bi bi-info-circle me-2 opacity-50"></i> Detail
                        </asp:LinkButton>
                        <asp:LinkButton runat="server" ID="btnDownloadQuote" CssClass="dropdown-item" Text="Download Quote" OnClick="btnDownloadQuote_Click" >
                            <i class="bi bi-cloud-arrow-down me-2 opacity-50"></i> Download
                        </asp:LinkButton>
                    </div>

                    <!-- button administrator -->
                    <button class="btn  btn-dark dropdown-toggle" data-bs-toggle="dropdown" runat="server" id="btnAdministrator">Admin</button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <asp:Button runat="server" ID="btnChangeStatus" CssClass="dropdown-item" Text="Change Status" OnClick="btnChangeStatus_Click" />
                        <asp:Button runat="server" ID="btnSendOrderMail" CssClass="dropdown-item" Text="Send Manual Order" OnClick="btnSendOrderMail_Click" />
                    </div>

                    <!-- button refresh pricing -->
                    <asp:LinkButton runat="server" ID="btnReloadPricing" CssClass="btn  btn-outline-indigo" Text="Print" OnClick="btnReloadPricing_Click" OnClientClick="return confirmReloadPricing();">
                        <i class="bi bi-credit-card-2-back me-2"></i> Reload Pricing
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
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
            <!-- msg success -->
            <div class="row" runat="server" id="divSuccess">
                <div class="col-lg-12">
                    <div class="alert alert-important alert-success alert-dismissible" role="alert">
                        <div class="d-flex">
                            <div>
                                <i class="bi bi-check-circle fs-3 me-2"></i>
                            </div>
                            <div>
                                <span runat="server" id="msgSuccess"></span>
                            </div>
                        </div>
                        <a class="btn-close" data-bs-dismiss="alert" aria-label="close"></a>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Jo Number :</span>
                                    <br />
                                    <div  runat="server" id="spanJoNumber"  ></div>
                                </div>

                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Order Number :</span>
                                    <br />
                                    <span  runat="server" id="spanOrderNo" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Reference :</span>
                                    <br />
                                    <span runat="server" id="spanOrderCust" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Created Date :</span>
                                    <br />
                                    <span runat="server" id="spanCreatedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Created By :</span>
                                    <br />
                                    <span runat="server" id="spanCreatedBy" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Note :</span>
                                    <br />
                                    <span runat="server" id="spanNote" style="font-size:small;"></span>
                                </div>

                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Status Note :</span>
                                    <br />
                                    <span runat="server" id="spanStatusNote" style="font-size:small;font-weight:bold;"></span>
                                </div>
                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Status Order :</span>
                                    <br />
                                    <span runat="server" id="spanStatusOrder" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Delivery / Pick Up :</span>
                                    <br />
                                    <span runat="server" id="spanDelivery" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-lg-6">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Submitted Date :</span>
                                    <br />
                                    <span runat="server" id="spanSubmittedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Completed Date :</span>
                                    <br />
                                    <span runat="server" id="spanCompletedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Canceled Date :</span>
                                    <br />
                                    <span runat="server" id="spanCanceledDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6" runat="server" id="divPrice">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Total excl. GST :</span>
                                    <br />
                                    <div class="" runat="server" id="spanTotal"></div>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">GST :</span>
                                    <br />
                                    <div class="" runat="server" id="spanGST" ></div>
                                </div>

                                 <div class="col-lg-4">
                                    <span style="font-size:larger;">TOTAL incl. GST :</span>
                                    <br />
                                    <div class="" runat="server" id="spanFinalTotal" ></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row mb-3">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">YOUR ITEMS</h3>
                            <div class="card-actions">
                                <a href="#" runat="server" id="aAddItem" class="btn btn-primary " data-bs-toggle="modal" data-bs-target="#modalAddItem">
                                    <i class="fa-solid fa-circle-plus" style="margin-right: 0.3rem;"></i> New Item
                                </a>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-vcenter table-striped table-hover card-table" AutoGenerateColumns="false" AllowPaging="True" PagerSettings-Position="Top" EmptyDataRowStyle-HorizontalAlign="Center" EmptyDataText="ORDER DATA NOT FOUND" PageSize="50" OnPageIndexChanging="gvList_PageIndexChanging" >
                                <RowStyle />
                                <Columns>
                                    <asp:TemplateField HeaderText="#">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="Item ID" ItemStyle-Width="90px" />
                                    <asp:BoundField DataField="Qty" HeaderText="Qty" ItemStyle-Width="60px" />
                                    <asp:BoundField DataField="Location" HeaderText="Location" ItemStyle-Width="120px" />
                                    <asp:TemplateField HeaderText="Product">
                                        <ItemTemplate>
                                            <%# BindItemDescription(Eval("Id").ToString()) %>
                                            <br />
                                            <asp:LinkButton runat="server" ID="linkNext" CssClass="btn btn-outline-success btn-sm badge-blink" Text='<%# TextNext(Eval("Id").ToString())%>' Visible='<%# VisibleNext(Eval("Id").ToString())%>' OnClick="linkNext_Click"></asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Cost">
                                        <ItemTemplate>
                                            <%# BindDetailCost(Eval("TotalMatrix"), Eval("TotalCharge")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mark Up">
                                        <ItemTemplate>
                                            <%# BindDetailMarkUp(Eval("MarkUp")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="210px">
                                        <ItemTemplate>
                                            <div class="float-end">
                                                <button class="opacity-50 dropdown-toggle border-0 bg-transparent" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                                    <i class="bi bi-three-dots-vertical fs-1"></i>
                                                </button>
                                                <ul class="dropdown-menu dropdown-menu-end">
                                                    <li>
                                                        <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Visible='<%# VisibleDetail()%>' OnClick="linkDetail_Click">
                                                            <i class="bi bi-info-circle me-2 opacity-50"></i>Detail
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton runat="server" ID="linkEdit" CssClass="dropdown-item" Visible='<%# VisibleEdit()%>' OnClick="linkEdit_Click">
                                                            <i class="bi bi-pencil-square me-2 opacity-50"></i>Edit
                                                        </asp:LinkButton>
                                                    </li>
                                                    <li>
                                                        <a href="javascript:void(0)" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalCopy" onclick='<%# String.Format("return showCopy(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("BracketType").ToString()) %>' visible='<%# VisibleCopy()%>'>
                                                            <i class="bi bi-copy me-2 opacity-50"></i>Copy
                                                        </a>
                                                    </li>
                                                    <li>
                                                        <a href="javascript:void(0)" runat="server" class="dropdown-item text-danger" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`, `{1}`);", Eval("Id").ToString(), "Detail") %>' visible='<%# VisibleDelete()%>'>
                                                            <i class="bi bi-trash3 me-2"></i>Delete
                                                        </a>
                                                    </li>
                                                    <li>
                                                        <asp:LinkButton runat="server" ID="linkPricing" CssClass="dropdown-item" Visible='<%# VisiblePricing()%>' OnClick="linkPricing_Click">
                                                            <i class="bi bi-tags me-2 opacity-50"></i>Pricing
                                                        </asp:LinkButton>
                                                    </li>
                                                </ul>
                                            </div>

                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <PagerStyle BackColor="Blue" ForeColor="White" HorizontalAlign="Center" />
                                <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                <AlternatingRowStyle BackColor="" />
                            </asp:GridView>
                        </div>
                        <div class="card-footer mt-2">
                            <p runat="server" id="pSubmit" class="text-secondary text-center"></p>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row" runat="server" id="divErrorB">
                <div class="col-lg-12">
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

    <div class="modal modal-blur fade" id="modalPricing" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
             <div class="modal-content">
                 <div class="modal-header">
                     <h5 class="modal-title">Cost Details</h5>
                     <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                 </div>
                 <div class="modal-body">
                     <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvPricing" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataRowStyle-HorizontalAlign="Center" EmptyDataText="DATA NOT FOUND">
                            <RowStyle />
                            <Columns>
                                <asp:TemplateField HeaderText="#" ItemStyle-Width="50px">
                                    <ItemTemplate>
                                        <%# Container.DataItemIndex + 1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Qty" HeaderText="Qty" />
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:BoundField DataField="FormatCost" HeaderText="Cost / Qty" />
                                <asp:BoundField DataField="FormatFinalCost" HeaderText="Sub Total" />
                            </Columns>
                            <PagerStyle BackColor="Blue" ForeColor="White" HorizontalAlign="Center" />
                            <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                            <AlternatingRowStyle BackColor="" />
                        </asp:GridView>
                    </div>
                 </div>
                 <div class="modal-footer">
                     <button type="button" class="btn btn-primary" data-bs-dismiss="modal">Close</button>
                 </div>
             </div>
        </div>
    </div>

    

    <div class="modal modal-blur fade" id="modalSubmit" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-primary"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-help-hexagon">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M19.875 6.27c.7 .398 1.13 1.143 1.125 1.948v7.284c0 .809 -.443 1.555 -1.158 1.948l-6.75 4.27a2.269 2.269 0 0 1 -2.184 0l-6.75 -4.27a2.225 2.225 0 0 1 -1.158 -1.948v-7.285c0 -.809 .443 -1.554 1.158 -1.947l6.75 -3.98a2.33 2.33 0 0 1 2.25 0l6.75 3.98h-.033z" />
                        <path d="M12 16v.01" />
                        <path d="M12 13a2 2 0 0 0 .914 -3.782a1.98 1.98 0 0 0 -2.414 .483" />
                    </svg>
                    <h3>Submit Order</h3>
                    <div class="text-secondary">
                        Are you sure you want to submit this order?<br /><br />
                        After orders being submitted,  this orders will automatically deliver to our system and any revisions is not allowed.
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary w-100" Text="YES. SUBMIT" OnClick="btnSubmit_Click" CommandName="Submit" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalNotif" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Order Information</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    You have an incomplete roller blinds order, which is on the ITEM ID <span id="spanInComplete"></span> <br /><br />
                    If you want to complete it, please click the <b>ADD BLIND</b> button on the order line ID.
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary" data-bs-dismiss="modal">OK</button>
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
                        <asp:TextBox runat="server" ID="txtActionDelete" style="display:none;"></asp:TextBox>
                        <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                        <span id="spanDescription"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger w-100" Text="Yes. I am" OnClick="btnDelete_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalCopy" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                <div class="modal-status bg-primary"></div>
                <div class="modal-body text-center py-4">
                    <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="icon icon-tabler icons-tabler-outline icon-tabler-copy">
                        <path stroke="none" d="M0 0h24v24H0z" fill="none" />
                        <path d="M7 7m0 2.667a2.667 2.667 0 0 1 2.667 -2.667h8.666a2.667 2.667 0 0 1 2.667 2.667v8.666a2.667 2.667 0 0 1 -2.667 2.667h-8.666a2.667 2.667 0 0 1 -2.667 -2.667z" />
                        <path d="M4.012 16.737a2.005 2.005 0 0 1 -1.012 -1.737v-10c0 -1.1 .9 -2 2 -2h10c.75 0 1.158 .385 1.5 1" />
                    </svg>
                    <h3>Are you sure?</h3>
                    <div class="text-secondary">
                        <asp:TextBox runat="server" ID="txtIdCopy" style="display:none;"></asp:TextBox>
                        Are you sure you want to copy this order?<br />
                        ITEM ID : <span id="spanIdCopy"></span><br /><br />
                        <span id="spanDescCopy"></span>
                    </div>
                </div>
                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col"><a href="#" class="btn w-100" data-bs-dismiss="modal">Cancel</a></div>
                            <div class="col">
                                <asp:Button runat="server" ID="btnCopyDetail" CssClass="btn btn-primary w-100" Text="Yes. I am" OnClick="btnCopyDetail_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalAddItem" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Add Item</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="row mb-3">
                        <div class="col-lg-12">
                            <label class="form-label required">SELECT PRODUCT</label>
                            <asp:DropDownList runat="server" ID="ddlDesign" CssClass="form-select "></asp:DropDownList>
                            <small class="form-hint" style="color:red;">* Please select a product then click the submit button</small>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <div class="w-100">
                        <div class="row">
                            <div class="col">
                                <a href="#" class="btn w-100 " data-bs-dismiss="modal">
                                    <i class="fa-solid fa-xmark me-2"></i>Cancel
                                </a>
                            </div>
                            <div class="col">
                                <asp:LinkButton runat="server" ID="btnSubmitAdd" CssClass="btn btn-primary w-100 " Text="Submit" OnClick="btnSubmitAdd_Click" >
                                    <i class="fa-solid fa-check me-2"></i>Submit
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- my custom script -->
    <script text="text/javascript" src="/Content/dist/js/my/order-detail.js"></script>
    <script type="text/javascript">
        $(function () {
            // $('#modalCopy').modal('show');           
        });

        setInterval(() => {
            fetch('/Account/KeepSessionAlive.aspx', {
                method: 'POST',
                credentials: 'include'
            }).then(res => {
                if (res.status === 401) {
                    // session expired, redirect to login?
                    window.location.href = '/Account/Login.aspx';
                }
                console.log(res.status);
            });
        }, 180000); // 3 menit

        // Function untuk menampilkan pesan error dari code-behind
        function showMessageError(msg){
            Swal.fire({
                icon: "error",
                title: "Oops...",
                html: msg,
            });
        }

    
        // Function untuk mengonfirmasi konversi order menjadi job
        function confirmConvertToJob() {
            Swal.fire({
                title: 'Are you sure?',
                text: "Do you want to convert this to a job?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, convert it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Jika dikonfirmasi, submit form untuk menjalankan event server-side
                    __doPostBack('<%= btnConvertToJob.UniqueID %>', '');
                }
            });
            return false; // Mencegah submit form secara langsung
        }
        // Function untuk mengonfirmasi konversi order menjadi job
        function confirmReloadPricing() {
            Swal.fire({
                title: 'Are you sure?',
                text: "Do you want to reload pricing?",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, reload it!'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Jika dikonfirmasi, submit form untuk menjalankan event server-side
                    __doPostBack('<%= btnReloadPricing.UniqueID %>', '');
                }
            });
            return false; // Mencegah submit form secara langsung
        }

        function showCopy(Id, BracketType) {
            document.getElementById('<%=txtIdCopy.ClientID %>').value = Id;
            document.getElementById("spanIdCopy").innerHTML = '<b>' + Id + '</b>';
            if (BracketType == 'Double' || BracketType == 'Linked 2 Blinds (Dep)' || BracketType == 'Linked 2 Blinds (Ind)' || BracketType == 'Linked 3 Blinds (Dep)' || BracketType == 'Linked 3 Blinds (Ind)') {
                var notes = 'This order type ' + BracketType + '';
                notes += '<br /><br />';
                notes += 'If you continue this process, the system will create a new one that is not connected to any line.';
                document.getElementById("spanDescCopy").innerHTML = notes;
            }
        }

        function showDelete(Id, Action) {
            var description = 'Are you sure you want to delete order <br />with <b>ITEM ID ' + Id + '</b> ? <br /> What you have done cannot be undone.';
            if (Action == 'Header') {
                var orderNo = document.getElementById('<%= spanOrderNo.ClientID %>').innerText;
                description = 'Are you sure you want to delete order <br /><b>Order Number : ' + orderNo + '</b> ? <br /> What you have done cannot be undone.';
            }
            document.getElementById('<%=txtActionDelete.ClientID %>').value = Action;
            document.getElementById('<%=txtIdDelete.ClientID %>').value = Id;
            document.getElementById("spanDescription").innerHTML = description;
        }

        function showDetailPrice(itemId, myProduct) {
            $('#modalPricing').modal('show');
        }
        function showConfirm(Data) {
            document.getElementById("spanInComplete").innerHTML = Data + '.';
            $('#modalNotif').modal('show');
        }
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblUserId"></asp:Label>

        <asp:Label runat="server" ID="lblItemId"></asp:Label>
        <asp:Label runat="server" ID="lblItemIdNew"></asp:Label>

        <asp:Label runat="server" ID="lblStoreId"></asp:Label>
        <asp:Label runat="server" ID="lblStoreType"></asp:Label>
        <asp:Label runat="server" ID="lblCompany"></asp:Label>

        <asp:Label runat="server" ID="lblOrderNo"></asp:Label>
        <asp:Label runat="server" ID="lblOrderCust"></asp:Label>

        <asp:Label runat="server" ID="lblUniqueId"></asp:Label>
        <asp:Label runat="server" ID="lblBlindNo"></asp:Label>
        <asp:Label runat="server" ID="lblBlindNoNew"></asp:Label>
        <asp:Label runat="server" ID="lblJoNumber"></asp:Label>



        <asp:SqlDataSource runat="server" ID="sdsCopy" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" InsertCommand="INSERT INTO OrderDetails SELECT @IdNew, HeaderId, KitId, SoeKitId, FabricId, ChainId, BottomRailId, PriceGroupId, CassetteExtraId, @UniqueId, BlindNo, Qty, Location, Mounting, Width, WidthB, WidthMiddle, WidthBottom, [Drop], DropB, DropMiddle, DropRight, SemiInsideMount, LouvreSize, LouvrePosition, HingeColour, MidrailHeight1, MidrailHeight2, MidrailCritical, Layout, LayoutSpecial, CustomHeaderLength, FrameType, FrameLeft, FrameRight, FrameTop, FrameBottom, BottomTrackType, BottomTrackRecess, Buildout, BuildoutPosition, PanelQty, TrackQty, PanelSize, NumOfPanel, HingeQtyPerPanel, PanelQtyWithHinge, LocationTPost1, LocationTPost2, LocationTPost3, LocationTPost4, LocationTPost5, HorizontalTPost, HorizontalTPostHeight, JoinedPanels, ReverseHinged, PelmetFlat, ExtraFascia, HingesLoose, TiltrodType, TiltrodSplit, SplitHeight1, SplitHeight2, DoorCutOut, SpecialShape, TemplateProvided, SquareMetre, LinearMetre, StackPosition, TilterPosition, RollDirection, ControlPosition, ControlColour, ControlLength, ChainLength, MaterialChain, MotorStyle, MotorRemote, MotorRequired, MotorBattery, MotorCharger, Connector, AdditionalMotor, CableExitPoint, TrackType, TrackColour, TrackLength, NumOfWand, WandPosition,  WandColour, WandLength, CordColour, CordLength, AcornPlasticColour, Accessory, SideBySide, SlatSize, SlatQty, TubeSize, Trim, Batten, BattenColour,  BracketOption, BracketColour, BracketCover, BracketExtension, Fitting, FlatType, ChildSafe, Cleat, BottomHoldDown, HangerType, PelmetType, PelmetWidth, PelmetSize, PelmetReturn, PelmetReturnPosition, PelmetReturnSize, PelmetReturnSize2, CutOut_LeftTop, CutOut_RightTop, CutOut_LeftBottom, CutOut_RightBottom, LHSWidth_Top, LHSHeight_Top, RHSWidth_Top, RHSHeight_Top, LHSWidth_Bottom, LHSHeight_Bottom, RHSWidth_Bottom, RHSHeight_Bottom, BlindSize, Sloper, InsertInTrack, Notes, Matrix, Charge, Discount, TotalMatrix, TotalCharge, TotalDiscount, MarkUp, Active FROM OrderDetails WHERE Id=@Id">
            <InsertParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblItemIdNew" Name="IdNew" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
            </InsertParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsHeader" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" DeleteCommand="UPDATE OrderHeaders SET Active=0 WHERE Id=@Id">
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsSubmit" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderHeaders SET Status='New Order',  SubmittedDate=GETDATE() WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblHeaderId" Name="Id" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsRestore" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET Active='1' WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsDetail" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET BlindNo=NULL WHERE Id=@Id" DeleteCommand="UPDATE OrderDetails SET Active=0 WHERE Id=@Id">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
            </UpdateParameters>
            <DeleteParameters>
                <asp:ControlParameter ControlID="lblItemId" Name="Id" PropertyName="Text" />
            </DeleteParameters>
        </asp:SqlDataSource>

        <asp:SqlDataSource runat="server" ID="sdsDetailBlindNo" ConnectionString="<%$ ConnectionStrings:DefaultConnection %>" UpdateCommand="UPDATE OrderDetails SET BlindNo=@BlindNoNew WHERE UniqueId=@UniqueId AND BlindNo=@BlindNo AND Active=1">
            <UpdateParameters>
                <asp:ControlParameter ControlID="lblBlindNoNew" Name="BlindNoNew" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblBlindNo" Name="BlindNo" PropertyName="Text" />
                <asp:ControlParameter ControlID="lblUniqueId" Name="UniqueId" PropertyName="Text" />
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>