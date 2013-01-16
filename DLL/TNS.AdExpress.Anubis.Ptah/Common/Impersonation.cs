﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace TNS.AdExpress.Anubis.Ptah.Common
{
    /// <summary>
    /// Impersonation
    /// </summary>
    public class Impersonation {

        private enum SecurityImpersonationLevel : int {
            /// <summary>
            /// The server process cannot obtain identification information about the client,
            /// and it cannot impersonate the client. It is defined with no value given, and thus,
            /// by ANSI C rules, defaults to a value of zero.
            /// </summary>
            SecurityAnonymous = 0,

            /// <summary>
            /// The server process can obtain information about the client, such as security identifiers and privileges,
            /// but it cannot impersonate the client. This is useful for servers that export their own objects,
            /// for example, database products that export tables and views.
            /// Using the retrieved client-security information, the server can make access-validation decisions without
            /// being able to use other services that are using the client's security context.
            /// </summary>
            SecurityIdentification = 1,

            /// <summary>
            /// The server process can impersonate the client's security context on its local system.
            /// The server cannot impersonate the client on remote systems.
            /// </summary>
            SecurityImpersonation = 2,

            /// <summary>
            /// The server process can impersonate the client's security context on remote systems.
            /// NOTE: Windows NT:  This impersonation level is not supported.
            /// </summary>
            SecurityDelegation = 3,
        }

        public enum LogonType : int {
            /// <summary>
            /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on  
            /// by a terminal server, remote shell, or similar process.
            /// This logon type has the additional expense of caching logon information for disconnected operations;
            /// therefore, it is inappropriate for some client/server applications,
            /// such as a mail server.
            /// </summary>
            LOGON32_LOGON_INTERACTIVE = 2,

            /// <summary>
            /// This logon type is intended for high performance servers to authenticate plaintext passwords.
            /// The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            LOGON32_LOGON_NETWORK = 3,

            /// <summary>
            /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without
            /// their direct intervention. This type is also for higher performance servers that process many plaintext
            /// authentication attempts at a time, such as mail or Web servers.
            /// The LogonUser function does not cache credentials for this logon type.
            /// </summary>
            LOGON32_LOGON_BATCH = 4,

            /// <summary>
            /// Indicates a service-type logon. The account provided must have the service privilege enabled.
            /// </summary>
            LOGON32_LOGON_SERVICE = 5,

            /// <summary>
            /// This logon type is for GINA DLLs that log on users who will be interactively using the computer.
            /// This logon type can generate a unique audit record that shows when the workstation was unlocked.
            /// </summary>
            LOGON32_LOGON_UNLOCK = 7,

            /// <summary>
            /// This logon type preserves the name and password in the authentication package, which allows the server to make
            /// connections to other network servers while impersonating the client. A server can accept plaintext credentials
            /// from a client, call LogonUser, verify that the user can access the system across the network, and still
            /// communicate with other servers.
            /// NOTE: Windows NT:  This value is not supported.
            /// </summary>
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8,

            /// <summary>
            /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
            /// The new logon session has the same local identifier but uses different credentials for other network connections.
            /// NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
            /// NOTE: Windows NT:  This value is not supported.
            /// </summary>
            LOGON32_LOGON_NEW_CREDENTIALS = 9,
        }

        private enum LogonProvider : int {
            /// <summary>
            /// Use the standard logon provider for the system.
            /// The default security provider is negotiate, unless you pass NULL for the domain name and the user name
            /// is not in UPN format. In this case, the default provider is NTLM.
            /// NOTE: Windows 2000/NT:   The default security provider is NTLM.
            /// </summary>
            LOGON32_PROVIDER_DEFAULT = 0,
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LogonUser(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            out IntPtr phToken
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        private extern static bool DuplicateToken(IntPtr ExistingTokenHandle, int
           SECURITY_IMPERSONATION_LEVEL, out IntPtr DuplicateTokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        private WindowsImpersonationContext _oWindowsImpersonationContext;
        private IntPtr _hToken = IntPtr.Zero;
        private IntPtr _hTokenDuplicate = IntPtr.Zero;

        public Impersonation() { }

        public bool ImpersonateValidUser(String userName, String domain, String password, LogonType eLogonType) {
            _hToken = IntPtr.Zero;
            _hTokenDuplicate = IntPtr.Zero;
            _oWindowsImpersonationContext = null;

            if (LogonUser(userName, domain, password,
                 (int)eLogonType,
                 (int)LogonProvider.LOGON32_PROVIDER_DEFAULT, out _hToken)) {
                if (DuplicateToken(_hToken, 2, out _hTokenDuplicate)) {
                    WindowsIdentity windowsIdentity = new WindowsIdentity(_hTokenDuplicate);
                    _oWindowsImpersonationContext = windowsIdentity.Impersonate();
                    windowsIdentity = null;

                    return true;
                }
            }
            return false;
        }

        public void UndoImpersonation() {
            if (_oWindowsImpersonationContext != null)
                _oWindowsImpersonationContext.Undo();

            if (_hToken != IntPtr.Zero) CloseHandle(_hToken);
            if (_hTokenDuplicate != IntPtr.Zero) CloseHandle(_hTokenDuplicate);
        }

    }
}
