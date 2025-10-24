<%@ Page Title="Detail Order" Language="VB" MasterPageFile="~/Site.master" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Order_Detail" MaintainScrollPositionOnPostback="true" Debug="true"%>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">


    
    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">

                <div class="col-lg-2 col-sm-12 col-md-12">
                    <div class="page-pretitle">Order</div>
                    <h2 class="page-title">Order Details</h2>
                </div>

                <div class="col-lg-10 col-sm-12 col-md-12 text-end">

                    <!-- buton finish -->
                    <button type="button" class="btn  btn-cyan" id="btnFinish">
                        <i class="bi bi-check-lg me-2"></i>
                        Finish
                    </button>

                    <!-- button preview -->
                    <button class="btn  btn-secondary dropdown-toggle" data-bs-toggle="dropdown">
                        <i class="bi bi-file-earmark-text me-2"></i>
                        Preview
                    </button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <li>
                            <a class="dropdown-item" href="javascript:void(0)" id="btnPreviewPrint">
                                <i class="bi bi-printer me-2 opacity-50"></i> Print
                            </a>
                        </li>
                        <li>
                            <a class="dropdown-item" href="javascript:void(0)" id="btnPreviewPDF">
                                    <i class="bi bi-file-earmark-pdf me-2 opacity-50"></i> Download PDF
                            </a>
                        </li>
                    </div>

                    <!-- button creat job sheet -->
                    <button class="btn btn-outline-teal dropdown-toggle " type="button" data-bs-toggle="dropdown" aria-expanded="false" id="btnJobSheet">
                        <i class="bi bi-file-text me-2"></i> Job Sheet
                    </button>
                    <ul class="dropdown-menu dropdown-menu-end">
                        <li>
                            <a href="javascript:void(0)" id="btnConvertToJob" class="dropdown-item">
                                <i class="bi bi-file-earmark-zip me-2 opacity-50"></i> Convert To Job
                            </a>
                        </li>
                        <li>
                            <a href="javascript:void(0)" id="btnReprintJobSheet" class="dropdown-item">
                                <i class="bi bi-printer me-2 opacity-50"></i> Reprint Job Sheet
                            </a>
                        </li>
                        <li>
                            <a href="javascript:void(0)" id="btnChangeJobStatus" class="dropdown-item" >
                                <i class="bi bi-clipboard-check me-2 opacity-50"></i> Change Job Status
                            </a>
                        </li>
                    </ul>

                    <!-- button submit -->
                    <a href="javascript:void(0)" id="btnSubmit" class="btn  btn-success">
                        <i class="fa-regular  fa-paper-plane me-2" ></i> Submit
                    </a>

                        <!-- button edit -->
                    <button type="button" id="btnEditHeader" class="btn  btn-primary">
                        <i class="fa-regular  fa-pen-to-square me-2" ></i> Edit
                    </button>

                    <!-- button delete -->
                    <a href="javascript:void(0)" id="btnDeleteHeader" class="btn  btn-danger">
                        <i class="fa-regular  fa-trash-can me-2" ></i> Delete
                    </a>

                    <!-- button quote -->
                    <button class="btn  btn-dark dropdown-toggle" data-bs-toggle="dropdown" id="btnQuote">
                        <i class="bi bi-chat-right-quote me-2" ></i> Quote
                    </button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <a href="javascript:void(0)" id="btnQuoteDetail" class="dropdown-item">
                            <i class="bi bi-info-circle me-2 opacity-50"></i> Detail
                        </a>
                        <a href="javascript:void(0)" id="btnDownloadQuote" class="dropdown-item">
                            <i class="bi bi-cloud-arrow-down me-2 opacity-50"></i> Download
                        </a>
                    </div>

                    <!-- button administrator -->
                    <button class="btn  btn-dark dropdown-toggle" data-bs-toggle="dropdown" id="btnAdministrator">Admin</button>
                    <div class="dropdown-menu dropdown-menu-end">
                        <a href="javascript:void(0)" id="btnChangeStatus" class="dropdown-item">
                            <i class="bi bi-clipboard-check me-2 opacity-50"></i> Change Status
                        </a>
                        <a href="javascript:void(0)" id="btnSendOrderMail" class="dropdown-item">
                            <i class="bi bi-send-fill me-2 opacity-50"></i> Send Manual Order
                        </a>
                    </div>

                        <!-- button refresh pricing -->
                    <button type="button" id="btnReloadPricing" class="btn  btn-outline-indigo">
                        <i class="bi bi-credit-card-2-back me-2"></i> Reload Pricing
                    </button>

                </div>

            </div>
        </div>
    </div><!-- /page-header -->

    <div class="page-body">
        <div class="container-xl">

            <!-- card information header 1-->
            <div class="row mb-3">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Jo Number :</span>
                                    <br />
                                    <div  id="spanJoNumber" style="font-size: larger;" ></div>
                                </div>

                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Order Number :</span>
                                    <br />
                                    <span  id="spanOrderNo" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Reference :</span>
                                    <br />
                                    <span id="spanOrderCust" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Created Date :</span>
                                    <br />
                                    <span id="spanCreatedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Created By :</span>
                                    <br />
                                    <span id="spanCreatedBy" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-2">
                                    <span style="font-size:larger;">Note :</span>
                                    <br />
                                    <span id="spanNote" style="font-size:small;"></span>
                                </div>

                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Status Note :</span>
                                    <br />
                                    <span id="spanStatusNote" style="font-size:small;font-weight:bold;"></span>
                                </div>
                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Status Order :</span>
                                    <br />
                                    <span id="spanStatusOrder" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-3">
                                    <span style="font-size:larger;">Delivery / Pick Up :</span>
                                    <br />
                                    <span id="spanDelivery" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div><!-- /card information header 1 -->

            <!-- card information header 2-->
            <div class="row mb-3">
                <div class="col-lg-6">
                    <div class="card">
                        <div class="card-body border-bottom py-3">
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Submitted Date :</span>
                                    <br />
                                    <span id="spanSubmittedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Completed Date :</span>
                                    <br />
                                    <span id="spanCompletedDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Canceled Date :</span>
                                    <br />
                                    <span id="spanCanceledDate" style="font-size:larger;font-weight:bold;"></span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6" id="divPrice">
                    <div class="card">
                        <div class="card-body border-bottom py-3" id="cardPrice">
                            <div class="row mb-4">
                                <div class="col-lg-4">
                                    <span style="font-size:larger;">Total excl. GST :</span>
                                    <br />
                                    <div  id="spanTotal"></div>
                                </div>

                                <div class="col-lg-4">
                                    <span style="font-size:larger;">GST :</span>
                                    <br />
                                    <div  id="spanGST" ></div>
                                </div>

                                 <div class="col-lg-4">
                                    <span style="font-size:larger;">TOTAL incl. GST :</span>
                                    <br />
                                    <div  id="spanFinalTotal" ></div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div><!-- /card information header 2 -->

            <!-- card table data items-->
            <div class="row mb-3">
                <div class="col-lg-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">YOUR ITEMS</h3>
                            <div class="card-actions">
                                <a href="javascript:void(0);" id="btnAddItem" class="btn btn-primary ">
                                    <i class="fa-solid fa-circle-plus" style="margin-right: 0.3rem;"></i> New Item
                                </a>
                            </div>
                        </div>
                        <div class="card-body">
                            <div id="table-default" class="">
                                <table class="table table-vcenter table-hover w-100" id="tableAjax">
                                 <thead>
                                     <tr>
                                         <th class="text-center">#</th>
                                         <th class="h3 text-center">ITEM ID</th>
                                         <th class="h3 text-center">QTY</th>
                                         <th class="h3">LOCATION</th>
                                         <th class="h3">PRODUCT</th>
                                         <th class="h3 ">COST</th>
                                         <th class="h3 thMarkUp" >MARK UP</th>
                                         <th class="h3 text-center">ACTIONS</th>
                                     </tr>
                                 </thead>
                                 <tbody></tbody>
                                </table>
                            </div>
                        </div>
                        <div class="card-footer">
                            <p id="msgThanks" class="text-secondary text-center">Thank you for submitting your order. Your order will be processed within 1 business day.<br /> Once your order has been processed, you can check the status from web order.<br /> Please do not fax us any paper work in addition to this online order as it may result in duplication.</p>
                        </div>
                    </div>
                </div>
            </div><!-- /card table data items-->

        </div>
    </div><!-- /page-body -->

    <!-- modalChangeStatus -->
    <div class="modal fade" id="modalChangeStatus" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalChangeStatusLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalChangeStatusLabel">Update Status Order</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body row">

                    <div class="col-12 mb-3">
                        <label for="status" class="form-label text-uppercase">Status</label>
                        <select name="status" id="status" class="form-select"></select>

                        <input type="text" class="form-control " id="statusOld" name="statusOld"readonly hidden>

                        <input type="text" class="form-control " id="id" name="id" placeholder="id for edit" readonly hidden>
                    </div>

                    <div class="col-12 mb-3" id="divSubmittedDate">
                        <label for="submitteddate" class="form-label text-uppercase">Submitted Date</label>
                        <input type="date" class="form-control" id="submitteddate" name="submitteddate">
                    </div>

                    <div class="col-12 mb-3" id="divCompletedDate">
                        <label for="completeddate" class="form-label text-uppercase">Completed Date</label>
                        <input type="date" class="form-control " id="completeddate" name="completeddate">
                    </div>

                    <div class="col-12 mb-3" id="divCanceledDate">
                        <label for="canceleddate" class="form-label text-uppercase">Canceled Date</label>
                        <input type="date" class="form-control " id="canceleddate" name="canceleddate">
                    </div>

                    <div class="col-12 mb-3" id="divDescription">
                        <label for="canceleddate" class="form-label text-uppercase d-flex justify-content-between">
                            Description
                            <i class="bi bi-question-circle" style="cursor: pointer;" id="tooltipDescription"></i>
                        </label>
                        <textarea name="description" class="form-control" id="description" rows="3" ></textarea>
                    </div>


                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                    <button type="button" class="btn btn-primary" id="submitChangeStatus">
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i>Submit
                    </button>
                </div>
            </div>
        </div>
    </div>


    <!-- modalAddItem -->
    <div class="modal fade" id="modalAddItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalAddItemLabel" aria-hidden="true">
        <div class="modal-dialog modal-sm modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalAddItemLabel">Add Item</h1>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body row">
                    <div class="col-12">
                        <label for="designid" class="form-label text-uppercase">select product <span class="text-danger">*</span></label>
                        <select name="designid" id="designid" class="form-select"></select>
                        <small class="form-hint" style="color:red;">* Please select a product then click the submit button</small>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                    <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                    <button type="button" class="btn btn-primary" id="submitAddItem">
                        <i class="fa-solid fa-cloud-arrow-up me-2"></i>Submit
                    </button>
                </div>
            </div>
        </div>
    </div>

    

    <!-- Modal -->
    <div class="modal fade" id="modalPricingItem" data-bs-backdrop="static" data-bs-keyboard="false" tabindex="-1" aria-labelledby="modalPricingItemLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h1 class="modal-title" id="modalPricingItemLabel">Cost Details</h1>
                    <button type="button" class="btn-close close-button" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">

                    <div id="table-default">
                        <table class="table table-vcenter card-table datatable" id="tablePricingDetail" width="100%">
                            <thead class="fs-3 fw-bold">
                                <tr>
                                    <th class="text-center">#</th>
                                    <th class="h3 text-center">Qty</th>
                                    <th class="h3">Descpription</th>
                                    <th class="h3">Cost / Qty</th>
                                    <th class="h3 ">Sub Total</th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary close-button" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-2"></i>Close
                    </button>
                </div>
            </div>
        </div>
    </div>

    <br>
    

    <script type="text/javascript">
        let userId = '<%= Session("userId") %>';
        let userName = '<%= Session("UserName") %>';
        let loginId = '<%= Session("LoginId") %>';
        let roleName = '<%= Session("RoleName") %>';  
        let storeId = '<%= Session("StoreId") %>';
        let storeCompany = '<%= Session("StoreCompany") %>';
        let headerId = '<%= Session("headerId") %>';
        let itemId = '<%= Session("itemId") %>';
        let levelName = '<%= Session("LevelName") %>';
        let pricesAccess = '<%= Session("PriceAccess") %>';
        let markupAccess = '<%= Session("MarkUpAccess") %>';
        let printPreview = '<%= Session("printPreview") %>';
        let Reprint = '<%= Session("Reprint") %>';
        let uriMethod = '/Methods/Order/DetailMethod.aspx';      
    </script>
    <script src="/Scripts/Order/Detail.js?<%= DateTime.Now.Ticks %>"></script>

</asp:Content>

