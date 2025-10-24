<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Import.aspx.vb" Inherits="Setting_PriceMatrix_Import" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Import Price Matrix" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        
        .rounded-start-pill {
        border-top-left-radius: 50rem !important;
        border-bottom-left-radius: 50rem !important;
        }

        .rounded-end-pill {
        border-top-right-radius: 50rem !important;
        border-bottom-right-radius: 50rem !important;
        }

        #loading-overlay {
            transition: opacity 1s ease;
            opacity: 1;
        }
        #loading-overlay.fade-out {
            opacity: 0;
        }
      

    </style>

    <!-- Loading Overlay -->
    <div id="loading-overlay" class="position-fixed top-0 start-0 w-100 h-100 d-flex justify-content-center align-items-center bg-body bg-opacity-85" style="z-index: 1050; display: none;">
        <div class="spinner-border text-primary" role="status" style="width: 3rem; height: 3rem;">
            <span class="visually-hidden">Loading...</span>
        </div>
    </div>


    <div class="page-header">
        <div class="container-xl">
            <div class="row g-2 align-items-center">
                <div class="col">
                    <div class="page-pretitle">Setting</div>
                    <h2 class="page-title">Price Matrix</h2>
                </div>
            </div>
        </div>
    </div>

    <div class="page-body">
        <div class="container-xl">
            <div class="row mb-3">
                <div class="col-lg-7 col-md-12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title" id="cardTitle">Import Price Matrix</h3>
                        </div>
                        <div class="card-body">
                            <div class="mb-3 row">
                                <label class="col-lg-2 col-form-label">File</label>
                                <div class="col-lg-10 col-md-12 col-sm-12">
                                    <input type="file" class="form-control " id="fileupload" name="fileupload" accept=".csv" />
                                </div>
                            </div>
                            <div class="mb-3 row">
                                <label class="col-lg-2 col-form-label">Price Group</label>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <select name="designid" id="designid" class="form-select "></select>
                                    <small class="form-hint">* Design Type</small>
                                </div>
                                <div class="col-lg-5 col-md-12 col-sm-12">
                                    <select name="pricegroupid" id="pricegroupid" class="form-select "></select>
                                    <small class="form-hint">* Price Group</small>
                                </div>
                            </div>
                            <div class="mb-3 row">
                                <label class="col-lg-2 col-form-label">Type</label>
                                <div class="col-lg-4 col-md-12 col-sm-12">
                                    <select name="type" id="type" class="form-select "></select>
                                </div>
                            </div>

                        </div>

                         <div class="card-footer text-end">
                            <button type="button" id="btnSubmit" class="btn btn-primary ">
                                <i class="fa-solid fa-cloud-arrow-up me-2"></i>
                                Submit
                            </button>
                            <a href="javascript:void(0);" id="btnCancel"  class="btn btn-danger " >
                                <i class="fa-solid fa-rotate-left me-2"></i> Cancel
                            </a>
                        </div>
                    </div>
                </div>

                <div class="col-lg-5 col-md12 col-sm-12">
                    <div class="card">
                        <div class="card-header">
                            <h3 class="card-title">Information !</h3>
                        </div>
                        <div class="card-body">
                            <p class="card-text">Note that the file extension that you will upload is <b>only .csv</b>, and make sure the csv format is in accordance with the example, you can download it using the download button below.</p>
                        </div>
                        <div class="card-footer text-end">
                            <a href="javascript:void(0);" id="btnDownloadExample" class="btn btn-primary ">
                                <i class="fa-solid fa-download me-2"></i>
                                Download
                            </a>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

    <script type="text/javascript">
        let userId = '<%= Session("userId") %>';
        let loginId = '<%= Session("LoginId") %>';
        let roleName = '<%= Session("RoleName") %>';
        
    </script>
    <script src="/Scripts/SettingPage/ImportMatrix.js?<%= DateTime.Now.Ticks %>"></script>
</asp:Content>