﻿//--------------------------------------------------------------------------
// <copyright file="UnsafeNativeMethods.cs" company="Jeff Winn">
//      Copyright (c) Jeff Winn. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      GNU Library General Public License (LGPL) v2.1 which can be found
//      in the License.rtf at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace DotRas.Internal
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains the unsafe remote access service (RAS) API function declarations.
    /// </summary>
    internal class UnsafeNativeMethods : IUnsafeNativeMethods
    {
        #region Fields

        /// <summary>
        /// Contains the instance used to handle calls.
        /// </summary>
        private static IUnsafeNativeMethods instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeNativeMethods"/> class.
        /// </summary>
        public UnsafeNativeMethods()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the instance of the <see cref="IUnsafeNativeMethods"/> class to handle calls.
        /// </summary>
        public static IUnsafeNativeMethods Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UnsafeNativeMethods();
                }

                return instance;
            }

            set
            {
                instance = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies a memory block from one location to another.
        /// </summary>
        /// <param name="destination">A pointer to the starting address of the move destination.</param>
        /// <param name="source">A pointer to the starting address of the block of memory to be moved.</param>
        /// <param name="length">The size of the memory block to move, in bytes.</param>
        public void CopyMemoryImpl(IntPtr destination, IntPtr source, IntPtr length)
        {
            UnsafeNativeMethods.CopyMemory(destination, source, length);
        }

        /// <summary>
        /// Deletes an entry from a phone book.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="entryName">The name of the entry to be deleted.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int DeleteEntry(string phoneBookPath, string entryName)
        {
            return UnsafeNativeMethods.RasDeleteEntry(phoneBookPath, entryName);
        }

        /// <summary>
        /// Establishes a remote access connection using a specified phone book entry. This function displays a stream of dialog boxes that indicate the state of the connection operation.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of an existing entry within the phone book.</param>
        /// <param name="phoneNumber">The phone number that overrides the numbers stored in the phone book entry.</param>
        /// <param name="info">A <see cref="NativeMethods.RASDIALDLG"/> structure containing input and output parameters.</param>
        /// <returns><b>true</b> if the function establishes a remote access connection, otherwise <b>false</b>.</returns>
        public bool DialDlg(string phoneBookPath, string entryName, string phoneNumber, ref NativeMethods.RASDIALDLG info)
        {
            return UnsafeNativeMethods.RasDialDlg(phoneBookPath, entryName, phoneNumber, ref info);
        }

#if (WINXP || WIN2K8 || WIN7)
        /// <summary>
        /// Deletes a subentry from the specified phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="entryName">The name of the entry to be deleted.</param>
        /// <param name="subEntryId">The one-based index of the subentry to delete.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int DeleteSubEntry(string phoneBookPath, string entryName, int subEntryId)
        {
            return UnsafeNativeMethods.RasDeleteSubEntry(phoneBookPath, entryName, subEntryId);
        }
#endif

        /// <summary>
        /// Displays a dialog box used to manipulate phone book entries.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of the entry to be created or modified.</param>
        /// <param name="info">An <see cref="NativeMethods.RASENTRYDLG"/> structure containing additional input/output parameters.</param>
        /// <returns><b>true</b> if the user creates, copies, or edits an entry, otherwise <b>false</b>.</returns>
        public bool EntryDlg(string phoneBookPath, string entryName, ref NativeMethods.RASENTRYDLG info)
        {
            return UnsafeNativeMethods.RasEntryDlg(phoneBookPath, entryName, ref info);
        }

        /// <summary>
        /// Lists all addresses in the AutoDial mapping database.
        /// </summary>
        /// <param name="value">An <see cref="StructBufferedPInvokeParams"/> containing call data.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>        
        public int EnumAutodialAddresses(StructBufferedPInvokeParams value)
        {
            IntPtr bufferSize = value.BufferSize;
            IntPtr count = value.Count;

            int ret = UnsafeNativeMethods.RasEnumAutodialAddresses(value.Address, ref bufferSize, ref count);
            value.BufferSize = bufferSize;
            value.Count = count;

            return ret;
        }

        /// <summary>
        /// Lists all entry names in a remote access phone-book.
        /// </summary>
        /// <param name="reserved">Reserved; this parameter must be a null reference.</param>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="entryName">Pointer to a buffer that, on output, receives an array of <see cref="NativeMethods.RASENTRYNAME"/> structures.</param>
        /// <param name="bufferSize">Upon return, contains the size in bytes of the buffer specified by <paramref name="entryName"/>. Upon return contains the number of bytes required to successfully complete the call.</param>
        /// <param name="count">Upon return, contains the number of phone book entries written to the buffer specified by <paramref name="entryName"/>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int EnumEntries(IntPtr reserved, string phoneBookPath, IntPtr entryName, ref IntPtr bufferSize, ref IntPtr count)
        {
            return UnsafeNativeMethods.RasEnumEntries(reserved, phoneBookPath, entryName, ref bufferSize, ref count);
        }

        /// <summary>
        /// Retrieves user credentials associated with a specified remote access phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="entryName">The name of an existing entry within the phone book.</param>
        /// <param name="credentials">Pointer to a <see cref="NativeMethods.RASCREDENTIALS"/> structure that upon return contains the requested credentials for the phone book entry.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int GetCredentials(string phoneBookPath, string entryName, IntPtr credentials)
        {
            return UnsafeNativeMethods.RasGetCredentials(phoneBookPath, entryName, credentials);
        }

        /// <summary>
        /// Retrieves information about the entries associated with a network address in the AutoDial mapping database.
        /// </summary>
        /// <param name="value">An <see cref="RasGetAutodialAddressParams"/> containing call data.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int GetAutodialAddress(RasGetAutodialAddressParams value)
        {
            IntPtr bufferSize = value.BufferSize;
            IntPtr count = value.Count;

            int ret = UnsafeNativeMethods.RasGetAutodialAddress(value.AutodialAddress, value.Reserved, value.Address, ref bufferSize, ref count);
            value.BufferSize = bufferSize;
            value.Count = count;

            return ret;
        }

        /// <summary>
        /// Indicates whether the AutoDial feature is enabled for a specific TAPI dialing location.
        /// </summary>
        /// <param name="value">An <see cref="RasGetAutodialEnableParams"/> containing call data.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int GetAutodialEnable(RasGetAutodialEnableParams value)
        {
            bool enabled = value.Enabled;
            
            int ret = UnsafeNativeMethods.RasGetAutodialEnable(value.DialingLocation, ref enabled);
            value.Enabled = enabled;

            return ret;
        }

        /// <summary>
        /// Retrieves the value of an AutoDial parameter.
        /// </summary>
        /// <param name="value">An <see cref="RasGetAutodialParamParams"/> containing call data.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int GetAutodialParam(RasGetAutodialParamParams value)
        {
            int bufferSize = value.BufferSize;

            int ret = UnsafeNativeMethods.RasGetAutodialParam(value.Key, value.Address, ref bufferSize);
            value.BufferSize = bufferSize;

            return ret;
        }

        /// <summary>
        /// Retrieves information for an existing phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of an existing entry within the phone book.</param>
        /// <param name="entry">Pointer to a buffer that, upon return, contains a <see cref="NativeMethods.RASENTRY"/> structure containing entry information.</param>
        /// <param name="bufferSize">Specifies the size of the <paramref name="entry"/> buffer.</param>
        /// <param name="deviceInfo">The parameter is not used.</param>
        /// <param name="deviceInfoSize">The parameter is not used.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int GetEntryProperties(string phoneBookPath, string entryName, IntPtr entry, ref IntPtr bufferSize, IntPtr deviceInfo, IntPtr deviceInfoSize)
        {
            return UnsafeNativeMethods.RasGetEntryProperties(phoneBookPath, entryName, entry, ref bufferSize, deviceInfo, deviceInfoSize);
        }

        /// <summary>
        /// Retrieves information about a subentry for the specified phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of an existing entry within the phone book.</param>
        /// <param name="index">The one-based index of the subentry to retrieve.</param>
        /// <param name="subentry">Pointer to a buffer that, upon return, contains a <see cref="NativeMethods.RASSUBENTRY"/> structure containing subentry information.</param>
        /// <param name="bufferSize">Upon return, contains the size in bytes of the buffer specified by <paramref name="subentry"/>. Upon return contains the number of bytes required to successfully complete the call.</param>
        /// <param name="deviceConfig">The parameter is not used.</param>
        /// <param name="deviceBufferSize">The parameter is not used.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int GetSubEntryProperties(string phoneBookPath, string entryName, int index, IntPtr subentry, ref IntPtr bufferSize, IntPtr deviceConfig, IntPtr deviceBufferSize)
        {
            return UnsafeNativeMethods.RasGetSubEntryProperties(phoneBookPath, entryName, index, subentry, ref bufferSize, deviceConfig, deviceBufferSize);
        }

        /// <summary>
        /// Displays the main dial-up networking dialog box.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="entryName">The name of the phone book entry to initially highlight.</param>
        /// <param name="info">An <see cref="NativeMethods.RASPBDLG"/> structure containing additional input/output parameters.</param>
        /// <returns><b>true</b> if the user dials an entry successfully, otherwise <b>false</b>.</returns>
        public bool PhonebookDlg(string phoneBookPath, string entryName, ref NativeMethods.RASPBDLG info)
        {
            return UnsafeNativeMethods.RasPhonebookDlg(phoneBookPath, entryName, ref info);
        }

        /// <summary>
        /// Renames an existing entry in a phone book.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="oldEntryName">The name of the entry to rename.</param>
        /// <param name="newEntryName">The new name of the entry.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int RenameEntry(string phoneBookPath, string oldEntryName, string newEntryName)
        {
            return UnsafeNativeMethods.RasRenameEntry(phoneBookPath, oldEntryName, newEntryName);
        }

        /// <summary>
        /// Updates an address in the AutoDial mapping database.
        /// </summary>
        /// <param name="address">The address for which information is being updated.</param>
        /// <param name="reserved">Reserved. This value must be zero.</param>
        /// <param name="addresses">Pointer to an array of <see cref="NativeMethods.RASAUTODIALENTRY"/> structures.</param>
        /// <param name="bufferSize">Upon return, contains the size in bytes of the buffer specified by <paramref name="addresses"/>. Upon return contains the number of bytes required to successfully complete the call.</param>
        /// <param name="count">Upon return, contains the number of phone book entries written to the buffer specified by <paramref name="addresses"/>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int SetAutodialAddress(string address, int reserved, IntPtr addresses, int bufferSize, int count)
        {
            return UnsafeNativeMethods.RasSetAutodialAddress(address, reserved, addresses, bufferSize, count);
        }

        /// <summary>
        /// Enables or disables the AutoDial feature for a specific TAPI dialing location.
        /// </summary>
        /// <param name="dialingLocation">The TAPI dialing location to update.</param>
        /// <param name="enabled"><b>true</b> to enable the AutoDial feature, otherwise <b>false</b> to disable it.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int SetAutodialEnable(int dialingLocation, bool enabled)
        {
            return UnsafeNativeMethods.RasSetAutodialEnable(dialingLocation, enabled);
        }

        /// <summary>
        /// Sets the value of an AutoDial parameter.
        /// </summary>
        /// <param name="key">The parameter whose value to set.</param>
        /// <param name="value">A pointer containing the new value of the parameter.</param>
        /// <param name="bufferSize">The size of the buffer.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int SetAutodialParam(NativeMethods.RASADP key, IntPtr value, int bufferSize)
        {
            return UnsafeNativeMethods.RasSetAutodialParam(key, value, bufferSize);
        }

        /// <summary>
        /// Sets the user credentials for a phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of the entry whose credentials to set.</param>
        /// <param name="credentials">Pointer to an <see cref="NativeMethods.RASCREDENTIALS"/> object containing user credentials.</param>
        /// <param name="clearCredentials"><b>true</b> clears existing credentials by setting them to an empty string, otherwise <b>false</b>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int SetCredentials(string phoneBookPath, string entryName, IntPtr credentials, bool clearCredentials)
        {
            return UnsafeNativeMethods.RasSetCredentials(phoneBookPath, entryName, credentials, clearCredentials);
        }

        /// <summary>
        /// Store user-specific Extensible Authentication Protocol (EAP) information for the specified phone book entry in the registry.
        /// </summary>
        /// <param name="handle">The handle to a primary or impersonation access token.</param>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The entry name to validate.</param>
        /// <param name="eapData">Pointer to a buffer that receives the retrieved EAP data for the user.</param>
        /// <param name="sizeOfEapData">On input specifies the size in bytes of the buffer pointed to by <paramref name="eapData"/>, upon output receives the size of the buffer needed to contain the EAP data.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int SetEapUserData(IntPtr handle, string phoneBookPath, string entryName, IntPtr eapData, int sizeOfEapData)
        {
            return UnsafeNativeMethods.RasSetEapUserData(handle, phoneBookPath, entryName, eapData, sizeOfEapData);
        }

        /// <summary>
        /// Sets the connection information for an entry within a phone book, or creates a new phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of an existing entry within the phone book.</param>
        /// <param name="entry">Pointer to a buffer that, upon return, contains a <see cref="NativeMethods.RASENTRY"/> structure containing entry information.</param>
        /// <param name="bufferSize">Specifies the size of the <paramref name="entry"/> buffer.</param>
        /// <param name="device">The parameter is not used.</param>
        /// <param name="deviceBufferSize">The parameter is not used.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int SetEntryProperties(string phoneBookPath, string entryName, IntPtr entry, int bufferSize, IntPtr device, int deviceBufferSize)
        {
            return UnsafeNativeMethods.RasSetEntryProperties(phoneBookPath, entryName, entry, bufferSize, device, deviceBufferSize);
        }

        /// <summary>
        /// Sets the subentry connection information of a specified phone book entry.
        /// </summary>
        /// <param name="phoneBookPath">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="entryName">The name of an existing entry within the phone book.</param>
        /// <param name="index">The one-based index of the subentry to set.</param>
        /// <param name="subentry">Pointer to a buffer that, upon return, contains a <see cref="NativeMethods.RASSUBENTRY"/> structure containing subentry information.</param>
        /// <param name="bufferSize">Specifies the size of the <paramref name="subentry"/> buffer.</param>
        /// <param name="deviceConfig">The parameter is not used.</param>
        /// <param name="deviceConfigSize">The parameter is not used.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int SetSubEntryProperties(string phoneBookPath, string entryName, int index, IntPtr subentry, int bufferSize, IntPtr deviceConfig, int deviceConfigSize)
        {
            return UnsafeNativeMethods.RasSetSubEntryProperties(phoneBookPath, entryName, index, subentry, bufferSize, deviceConfig, deviceConfigSize);
        }

#if (WIN7)
        /// <summary>
        /// Updates the tunnel endpoints of an Internet Key Exchange (IKEv2) connection.
        /// </summary>
        /// <param name="handle">The handle to the connection.</param>
        /// <param name="updateData">Pointer to a <see cref="NativeMethods.RASUPDATECONN"/> structure that contains the new tunnel endpoints for the connection.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        public int UpdateConnection(RasHandle handle, IntPtr updateData)
        {
            return UnsafeNativeMethods.RasUpdateConnection(handle, updateData);
        }
#endif

        /// <summary>
        /// Copies a memory block from one location to another.
        /// </summary>
        /// <param name="destination">A pointer to the starting address of the move destination.</param>
        /// <param name="source">A pointer to the starting address of the block of memory to be moved.</param>
        /// <param name="length">The size of the memory block to move, in bytes.</param>
        [DllImport("kernel32.dll")]
        private static extern void CopyMemory(
            IntPtr destination,
            IntPtr source,
            IntPtr length);

        /// <summary>
        /// Deletes an entry from a phone book.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of the entry to be deleted.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasDeleteEntry(
            string lpszPhonebook,
            string lpszEntryName);

#if (WINXP || WIN2K8 || WIN7)
        /// <summary>
        /// Deletes a subentry from the specified phone book entry.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of the entry to be deleted.</param>
        /// <param name="subEntryId">The one-based index of the subentry to delete.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasDeleteSubEntry(
            string lpszPhonebook,
            string lpszEntryName,
            int subEntryId);
#endif

        /// <summary>
        /// Establishes a remote access connection using a specified phone book entry. This function displays a stream of dialog boxes that indicate the state of the connection operation.
        /// </summary>
        /// <param name="lpszPhoneBook">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of an existing entry within the phone book.</param>
        /// <param name="lpszPhoneNumber">The phone number that overrides the numbers stored in the phone book entry.</param>
        /// <param name="lpInfo">A <see cref="NativeMethods.RASDIALDLG"/> structure containing input and output parameters.</param>
        /// <returns><b>true</b> if the function establishes a remote access connection, otherwise <b>false</b>.</returns>
        [DllImport("rasdlg.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RasDialDlg(
            string lpszPhoneBook,
            string lpszEntryName,
            string lpszPhoneNumber,
            ref NativeMethods.RASDIALDLG lpInfo);

        /// <summary>
        /// Displays a dialog box used to manipulate phone book entries.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of the entry to be created or modified.</param>
        /// <param name="lpInfo">An <see cref="NativeMethods.RASENTRYDLG"/> structure containing additional input/output parameters.</param>
        /// <returns><b>true</b> if the user creates, copies, or edits an entry, otherwise <b>false</b>.</returns>
        [DllImport("rasdlg.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RasEntryDlg(
            string lpszPhonebook,
            string lpszEntryName,
            ref NativeMethods.RASENTRYDLG lpInfo);

        /// <summary>
        /// Lists all addresses in the AutoDial mapping database.
        /// </summary>
        /// <param name="lppAddresses">Pointer to a buffer that, on output, receives an array of <see cref="NativeMethods.RASAUTODIALENTRY"/> structures.</param>
        /// <param name="lpCb">Upon return, contains the size in bytes of the buffer specified by <paramref name="lppAddresses"/>. Upon return contains the number of bytes required to successfully complete the call.</param>
        /// <param name="lpcAddresses">Upon return, contains the number of address strings written to the buffer specified by <paramref name="lppAddresses"/>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasEnumAutodialAddresses(
            [In, Out] IntPtr lppAddresses,
            ref IntPtr lpCb,
            ref IntPtr lpcAddresses);

        /// <summary>
        /// Lists all entry names in a remote access phone-book.
        /// </summary>
        /// <param name="reserved">Reserved; this parameter must be a null reference.</param>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="lpRasEntryName">Pointer to a buffer that, on output, receives an array of <see cref="NativeMethods.RASENTRYNAME"/> structures.</param>
        /// <param name="lpCb">Upon return, contains the size in bytes of the buffer specified by <paramref name="lpRasEntryName"/>. Upon return contains the number of bytes required to successfully complete the call.</param>
        /// <param name="lpcEntries">Upon return, contains the number of phone book entries written to the buffer specified by <paramref name="lpRasEntryName"/>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasEnumEntries(
            IntPtr reserved,
            string lpszPhonebook,
            [In, Out] IntPtr lpRasEntryName,
            ref IntPtr lpCb,
            ref IntPtr lpcEntries);

        /// <summary>
        /// Retrieves information about the entries associated with a network address in the AutoDial mapping database.
        /// </summary>
        /// <param name="lpszAddress">The address for which information is being requested.</param>
        /// <param name="lpdwReserved">Reserved. This argument must be zero.</param>
        /// <param name="lpAddresses">Pointer to a buffer that, on output, receives an array of <see cref="NativeMethods.RASAUTODIALENTRY"/> structures.</param>
        /// <param name="lpCb">Upon return, contains the size in bytes of the buffer specified by <paramref name="lpAddresses"/>. Upon return contains the number of bytes required to successfully complete the call.</param>
        /// <param name="lpcEntries">Upon return, contains the number of phone book entries written to the buffer specified by <paramref name="lpAddresses"/>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasGetAutodialAddress(
            string lpszAddress,
            IntPtr lpdwReserved,
            [In, Out] IntPtr lpAddresses,
            ref IntPtr lpCb,
            ref IntPtr lpcEntries);

        /// <summary>
        /// Indicates whether the AutoDial feature is enabled for a specific TAPI dialing location.
        /// </summary>
        /// <param name="dwDialingLocation">The identifier of the TAPI dialing location.</param>
        /// <param name="lpfEnabled">Pointer to a <see cref="System.Boolean"/> that upon return indicates whether AutoDial is enabled for the specified dialing location.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasGetAutodialEnable(
            int dwDialingLocation,
            [MarshalAs(UnmanagedType.Bool)]
            ref bool lpfEnabled);

        /// <summary>
        /// Retrieves the value of an AutoDial parameter.
        /// </summary>
        /// <param name="dwKey">The AutoDial parameter to retrieve.</param>
        /// <param name="lpvValue">Pointer to a buffer that receives the value for the specified parameter.</param>
        /// <param name="lpdwcbValue">On input, contains the size, in bytes, of the <paramref name="lpvValue"/> buffer. Upon return, contains the actual size of the value written to the buffer.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasGetAutodialParam(
            NativeMethods.RASADP dwKey,
            IntPtr lpvValue,
            ref int lpdwcbValue);

        /// <summary>
        /// Retrieves user credentials associated with a specified remote access phone book entry.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of an existing entry within the phone book.</param>
        /// <param name="lpCredentials">Pointer to a <see cref="NativeMethods.RASCREDENTIALS"/> structure that upon return contains the requested credentials for the phone book entry.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasGetCredentials(
            string lpszPhonebook,
            string lpszEntryName,
            [In, Out] IntPtr lpCredentials);

        /// <summary>
        /// Retrieves information for an existing phone book entry.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of an existing entry within the phone book.</param>
        /// <param name="lpRasEntry">Pointer to a buffer that, upon return, contains a <see cref="NativeMethods.RASENTRY"/> structure containing entry information.</param>
        /// <param name="dwEntryInfoSize">Specifies the size of the <paramref name="lpRasEntry"/> buffer.</param>
        /// <param name="lpbDeviceInfo">The parameter is not used.</param>
        /// <param name="dwDeviceInfoSize">The parameter is not used.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasGetEntryProperties(
            string lpszPhonebook,
            string lpszEntryName,
            [In, Out] IntPtr lpRasEntry,
            ref IntPtr dwEntryInfoSize,
            IntPtr lpbDeviceInfo,
            IntPtr dwDeviceInfoSize);

        /// <summary>
        /// Retrieves information about a subentry for the specified phone book entry.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of an existing entry within the phone book.</param>
        /// <param name="dwSubEntry">The one-based index of the subentry to retrieve.</param>
        /// <param name="lpRasSubEntry">Pointer to a buffer that, upon return, contains a <see cref="NativeMethods.RASSUBENTRY"/> structure containing subentry information.</param>
        /// <param name="lpCb">Upon return, contains the size in bytes of the buffer specified by <paramref name="lpRasSubEntry"/>. Upon return contains the number of bytes required to successfully complete the call.</param>
        /// <param name="lpbDeviceConfig">The parameter is not used.</param>
        /// <param name="lpcbDeviceConfig">The parameter is not used.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasGetSubEntryProperties(
            string lpszPhonebook,
            string lpszEntryName,
            int dwSubEntry,
            [In, Out] IntPtr lpRasSubEntry,
            ref IntPtr lpCb,
            IntPtr lpbDeviceConfig,
            IntPtr lpcbDeviceConfig);

        /// <summary>
        /// Displays the main dial-up networking dialog box.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference, the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of the phone book entry to initially highlight.</param>
        /// <param name="lpInfo">An <see cref="NativeMethods.RASPBDLG"/> structure containing additional input/output parameters.</param>
        /// <returns><b>true</b> if the user dials an entry successfully, otherwise <b>false</b>.</returns>
        [DllImport("rasdlg.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RasPhonebookDlg(
            string lpszPhonebook,
            string lpszEntryName,
            ref NativeMethods.RASPBDLG lpInfo);

        /// <summary>
        /// Renames an existing entry in a phone book.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszOldEntryName">The name of the entry to rename.</param>
        /// <param name="lpszNewEntryName">The new name of the entry.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasRenameEntry(
            string lpszPhonebook,
            string lpszOldEntryName,
            string lpszNewEntryName);

        /// <summary>
        /// Updates an address in the AutoDial mapping database.
        /// </summary>
        /// <param name="lpszAddress">The address for which information is being updated.</param>
        /// <param name="dwReserved">Reserved. This value must be zero.</param>
        /// <param name="lppAddresses">Pointer to an array of <see cref="NativeMethods.RASAUTODIALENTRY"/> structures.</param>
        /// <param name="lpCb">Upon return, contains the size in bytes of the buffer specified by <paramref name="lppAddresses"/>. Upon return contains the number of bytes required to successfully complete the call.</param>
        /// <param name="lpcEntries">Upon return, contains the number of phone book entries written to the buffer specified by <paramref name="lppAddresses"/>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasSetAutodialAddress(
            string lpszAddress,
            int dwReserved,
            IntPtr lppAddresses,
            int lpCb,
            int lpcEntries);

        /// <summary>
        /// Enables or disables the AutoDial feature for a specific TAPI dialing location.
        /// </summary>
        /// <param name="dwDialingLocation">The TAPI dialing location to update.</param>
        /// <param name="fEnabled"><b>true</b> to enable the AutoDial feature, otherwise <b>false</b> to disable it.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasSetAutodialEnable(
            int dwDialingLocation,
            [MarshalAs(UnmanagedType.Bool)]
            bool fEnabled);

        /// <summary>
        /// Sets the value of an AutoDial parameter.
        /// </summary>
        /// <param name="dwKey">The parameter whose value to set.</param>
        /// <param name="lpvValue">A pointer containing the new value of the parameter.</param>
        /// <param name="dwcbValue">The size of the buffer.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasSetAutodialParam(
            NativeMethods.RASADP dwKey,
            IntPtr lpvValue,
            int dwcbValue);

        /// <summary>
        /// Sets the user credentials for a phone book entry.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of the entry whose credentials to set.</param>
        /// <param name="lpCredentials">Pointer to an <see cref="NativeMethods.RASCREDENTIALS"/> object containing user credentials.</param>
        /// <param name="fClearCredentials"><b>true</b> clears existing credentials by setting them to an empty string, otherwise <b>false</b>.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasSetCredentials(
            string lpszPhonebook,
            string lpszEntryName,
            IntPtr lpCredentials,
            [MarshalAs(UnmanagedType.Bool)] bool fClearCredentials);

        /// <summary>
        /// Store user-specific Extensible Authentication Protocol (EAP) information for the specified phone book entry in the registry.
        /// </summary>
        /// <param name="handle">The handle to a primary or impersonation access token.</param>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The entry name to validate.</param>
        /// <param name="lpbEapData">Pointer to a buffer that receives the retrieved EAP data for the user.</param>
        /// <param name="dwSizeOfEapData">On input specifies the size in bytes of the buffer pointed to by <paramref name="lpbEapData"/>, upon output receives the size of the buffer needed to contain the EAP data.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasSetEapUserData(
            IntPtr handle,
            string lpszPhonebook,
            string lpszEntryName,
            IntPtr lpbEapData,
            int dwSizeOfEapData);

        /// <summary>
        /// Sets the connection information for an entry within a phone book, or creates a new phone book entry.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of an existing entry within the phone book.</param>
        /// <param name="lpRasEntry">Pointer to a buffer that, upon return, contains a <see cref="NativeMethods.RASENTRY"/> structure containing entry information.</param>
        /// <param name="dwEntryInfoSize">Specifies the size of the <paramref name="lpRasEntry"/> buffer.</param>
        /// <param name="lpbDeviceInfo">The parameter is not used.</param>
        /// <param name="dwDeviceInfoSize">The parameter is not used.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasSetEntryProperties(
            string lpszPhonebook,
            string lpszEntryName,
            IntPtr lpRasEntry,
            int dwEntryInfoSize,
            IntPtr lpbDeviceInfo,
            int dwDeviceInfoSize);

        /// <summary>
        /// Sets the subentry connection information of a specified phone book entry.
        /// </summary>
        /// <param name="lpszPhonebook">The full path and filename of a phone book file. If this parameter is a null reference (<b>Nothing</b> in Visual Basic), the default phone book is used.</param>
        /// <param name="lpszEntryName">The name of an existing entry within the phone book.</param>
        /// <param name="dwSubEntry">The one-based index of the subentry to set.</param>
        /// <param name="lpRasSubEntry">Pointer to a buffer that, upon return, contains a <see cref="NativeMethods.RASSUBENTRY"/> structure containing subentry information.</param>
        /// <param name="dwcbRasSubEntry">Specifies the size of the <paramref name="lpRasSubEntry"/> buffer.</param>
        /// <param name="lpbDeviceConfig">The parameter is not used.</param>
        /// <param name="dwcbDeviceConfig">The parameter is not used.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasSetSubEntryProperties(
            string lpszPhonebook,
            string lpszEntryName,
            int dwSubEntry,
            IntPtr lpRasSubEntry,
            int dwcbRasSubEntry,
            IntPtr lpbDeviceConfig,
            int dwcbDeviceConfig);

#if (WIN7)
        /// <summary>
        /// Updates the tunnel endpoints of an Internet Key Exchange (IKEv2) connection.
        /// </summary>
        /// <param name="hRasConn">The handle to the connection.</param>
        /// <param name="lpRasUpdateConn">Pointer to a <see cref="NativeMethods.RASUPDATECONN"/> structure that contains the new tunnel endpoints for the connection.</param>
        /// <returns>If the function succeeds, the return value is zero.</returns>
        [DllImport("rasapi32.dll", CharSet = CharSet.Unicode)]
        private static extern int RasUpdateConnection(
            RasHandle hRasConn,
            IntPtr lpRasUpdateConn);
#endif

        #endregion
    }
}