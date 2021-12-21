﻿/*
    Kryptor: A simple, modern, and secure encryption tool.
    Copyright (C) 2020-2022 Samuel Lucas

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.IO;
using System.Text;
using Sodium;

namespace KryptorCLI;

public static class FileHeaders
{
    public static void WriteHeaders(FileStream outputFile, byte[] ephemeralPublicKey, byte[] salt, byte[] nonce, byte[] encryptedHeader)
    {
        const int offset = 0;
        outputFile.Write(Constants.KryptorMagicBytes, offset, Constants.KryptorMagicBytes.Length);
        outputFile.Write(Constants.EncryptionVersion, offset, Constants.EncryptionVersion.Length);
        outputFile.Write(ephemeralPublicKey, offset, ephemeralPublicKey.Length);
        outputFile.Write(salt, offset, salt.Length);
        outputFile.Write(nonce, offset, nonce.Length);
        outputFile.Write(encryptedHeader, offset, encryptedHeader.Length);
    }

    public static byte[] GetFileNameLength(string inputFilePath, bool zeroByteFile)
    {
        if (!zeroByteFile && !Globals.ObfuscateFileNames) { return BitConversion.GetBytes(0); }
        var fileNameBytes = Encoding.UTF8.GetBytes(Path.GetFileName(inputFilePath));
        return BitConversion.GetBytes(fileNameBytes.Length);
    }

    public static void ValidateFormatVersion(byte[] formatVersion, byte[] currentFormatVersion)
    {
        bool validFormatVersion = Utilities.Compare(formatVersion, currentFormatVersion);
        if (!validFormatVersion) { throw new ArgumentException("Incorrect file format for this version of Kryptor."); }
    }

    public static byte[] ReadEphemeralPublicKey(FileStream inputFile)
    {
        int offset = Constants.KryptorMagicBytes.Length + Constants.EncryptionVersion.Length;
        return FileHandling.ReadFileHeader(inputFile, offset, Constants.EphemeralPublicKeyLength);
    }

    public static byte[] ReadSalt(FileStream inputFile)
    {
        int offset = Constants.KryptorMagicBytes.Length + Constants.EncryptionVersion.Length + Constants.EphemeralPublicKeyLength;
        return FileHandling.ReadFileHeader(inputFile, offset, Constants.SaltLength);
    }

    public static byte[] ReadNonce(FileStream inputFile)
    {
        int offset = Constants.KryptorMagicBytes.Length + Constants.EncryptionVersion.Length + Constants.EphemeralPublicKeyLength + Constants.SaltLength;
        return FileHandling.ReadFileHeader(inputFile, offset, Constants.XChaChaNonceLength);
    }

    public static byte[] ReadEncryptedHeader(FileStream inputFile)
    {
        int offset = Constants.KryptorMagicBytes.Length + Constants.EncryptionVersion.Length + Constants.EphemeralPublicKeyLength + Constants.SaltLength + Constants.XChaChaNonceLength;
        return FileHandling.ReadFileHeader(inputFile, offset, Constants.EncryptedHeaderLength);
    }
}