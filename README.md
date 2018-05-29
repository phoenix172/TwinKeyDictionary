# TwinKeyDictionary

A dictionary, with one primary and one secondary key. It implements IDictionary<TKeyPrimary, TValue>, so that it can be used as a regular dictionary, in which case the secondary key is ignored.

Searches can be done using either both keys, or just the primary key. If there are multiple items with the same primary key but with different secondary keys, the first one is returned.
