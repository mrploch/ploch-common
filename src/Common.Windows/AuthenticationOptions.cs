﻿/*
Copyright 2017 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

namespace Ploch.Common.Windows
{
    /// <summary>
    ///     Authentication options
    /// </summary>
    public class AuthenticationOptions
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="AuthenticationOptions" /> is impersonate.
        /// </summary>
        /// <value><c>true</c> if impersonate; otherwise, <c>false</c>.</value>
        public bool Impersonate { get; set; }

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        ///     Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
    }
}